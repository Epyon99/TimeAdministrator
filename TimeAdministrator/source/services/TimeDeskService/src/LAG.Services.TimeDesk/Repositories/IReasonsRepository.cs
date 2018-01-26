using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EPY.Services.LogTiempo.Repositories.Models;

namespace EPY.Services.LogTiempo.Repositories
{
    public interface IReasonsRepository
    {
        /// <summary>
        /// Returns a collection of reasons ordered by its weight
        /// </summary>
        /// <param name="user">User to find</param>
        /// <returns>List of Reasons of the user</returns>
        Task<UserReasons> GetTopUserReasons(string user);

        /// <summary>
        /// Returns all reasons for an user
        /// </summary>
        /// <param name="user">The user to filter</param>
        /// <returns>The list with all the reasons</returns>
        Task<UserReasons> GetAllUserReasons(string user);

        /// <summary>
        /// Finds and Replaces an UserReasons
        /// </summary>
        /// <param name="userReason">The userreasons to update</param>
        /// <returns>Void</returns>
        Task UpdateUserReason(UserReasons userReason);

        /// <summary>
        /// Adds a new UserReason to the collection.
        /// </summary>
        /// <param name="userReason">An unique userReason</param>
        /// <returns>Void</returns>
        Task AddUserReason(UserReasons userReason);
    }
}
