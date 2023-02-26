using API.DTOs;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class FollowsController : BaseApiController
    {
        private readonly IUnitOfWork unitOfWork;

        public FollowsController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddFollow(string username)
        {
            var sourceUserId = User.GetUserId();
            var followedUser= await unitOfWork.UserRepository.GetUserByUsernameAsync(username);
            var sourceUser = await unitOfWork.FollowsRepository.GetUserWithFollows(sourceUserId);
            if (followedUser == null) return NotFound();
            if (sourceUser.UserName == username) return BadRequest("You cannot follow yourself!");
            var userFollow = await unitOfWork.FollowsRepository.GetUserFollow(sourceUserId, followedUser.Id);
            if (userFollow != null) return BadRequest("you already follow this user");
            userFollow = new Entities.UserFollow
            {
                SourceUserId = sourceUserId,
                FollowedUserId = followedUser.Id,
            };
            sourceUser.FollowedUsers.Add(userFollow);
            if (await unitOfWork.Complete()) return Ok();
            return BadRequest("failed to follow user");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FollowDto>>> GetUserFollows([FromQuery]FollowsParams followsParams)
        {
            followsParams.UserId = User.GetUserId();
            var users =  await unitOfWork.FollowsRepository.GetUserFollows(followsParams);
            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
            return Ok(users);
        }
    }
}
