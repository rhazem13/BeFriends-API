using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class FollowsRepository : IFollowsRepository
    {
        private readonly DataContext context;

        public FollowsRepository(DataContext context)
        {
            this.context = context;
        }
        public async Task<UserFollow> GetUserFollow(int sourceUserId, int followedUserId)
        {
            return await context.Follows.FindAsync(sourceUserId, followedUserId);
        }

        public async Task<PagedList<FollowDto>> GetUserFollows(FollowsParams followsParams)
        {
            var users = context.Users.OrderBy(u => u.UserName).AsQueryable();
            var follows = context.Follows.AsQueryable();

            if(followsParams.Predicate== "followed")
            {
                follows = follows.Where(follow => follow.SourceUserId == followsParams.UserId);
                users = follows.Select(follow => follow.FollowedUser);
            }

            if(followsParams.Predicate == "followedBy")
            {
                follows = follows.Where(follow => follow.FollowedUserId == followsParams.UserId);
                users = follows.Select(follow => follow.SourceUser);
            }

            var followedUsers = users.Select(user => new FollowDto
            {
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
                City = user.City,
                Id = user.Id
            });
            return await PagedList<FollowDto>.CreateAsync(followedUsers, followsParams.PageNumber,followsParams.PageSize);
        }

        public async Task<AppUser> GetUserWithFollows(int userId)
        {
            return await context.Users.Include(x => x.FollowedUsers).FirstOrDefaultAsync(x => x.Id == userId);
        }
    }
}
