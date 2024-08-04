using AutoMapper;
using EtaAnnouncer.Contracts;
using EtaAnnouncer.Data;
using EtaAnnouncer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EtaAnnouncer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController(UserManager<Member> userManager, ApplicationDbContext dbContext, IMapper mapper) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var member = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            if (member == null)
            {
                return Unauthorized();
            }
            var homes = await dbContext.Homes.AsNoTracking()
                                             .Where(h => h.Members.Any(m => m.Id == member.Id))
                                             .Include(h => h.Members)
                                             .ToListAsync();
            return Ok(mapper.Map<List<HomeDto>>(homes));
        }

        public record CreateHomeRequest(string Name);
        [HttpPost("create")]
        public async Task<IActionResult> CreateHome([FromBody] CreateHomeRequest homeDto)
        {
            var member = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            if (member == null)
            {
                return Unauthorized();
            }
            var home = await dbContext.Homes.AddAsync(new Home { Name = homeDto.Name, Members = [member] });
            await dbContext.SaveChangesAsync();
            return Created("api/home", home.Entity.Id);
        }

        public record CreateInviteRequest(string HomeId);
        [HttpPost("invite")]
        public async Task<IActionResult> CreateInvite([FromBody] CreateInviteRequest createInviteRequest)
        {
            var member = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            if (member == null)
            {
                return Unauthorized();
            }

            var home = await dbContext.Homes
                .Include(h => h.Members)
                .FirstOrDefaultAsync(h => h.Id.ToString() == createInviteRequest.HomeId && h.Members.Any(m => m.Id == member.Id));
            if (home == null)
            {
                return Unauthorized();
            }

            var invitation = await dbContext.Invitations.AddAsync(new HomeInvitation { Home = home, ExpiryDate = DateTime.UtcNow.AddDays(7) });
            await dbContext.SaveChangesAsync();
            return Created("api/home", invitation.Entity.Id);
        }

        public record AddMemberRequest(string InvitationId);
        [HttpPut]
        public async Task<IActionResult> AddMember([FromBody] AddMemberRequest addMemberRequest)
        {
            var invitation = await dbContext.Invitations
                .AsNoTracking()
                .Include(i => i.Home)
                .FirstOrDefaultAsync(i => !i.IsRevoked && i.ExpiryDate > DateTime.UtcNow && i.Id.ToString() == addMemberRequest.InvitationId);

            if (invitation == null)
            {
                return NotFound();
            }

            var home = await dbContext.Homes
                .Include(h => h.Members)
                .FirstOrDefaultAsync(h => h.Id == invitation.Home.Id);
            if (home == null)
            {
                return NotFound();
            }

            var member = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            if (member == null)
            {
                return Unauthorized();
            }
            if (home.Members.Exists(m => m.Id == member.Id))
            {
                return Conflict();
            }
            home.Members.Add(member);
            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
