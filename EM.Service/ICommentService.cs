using EM.Domain.Entities;
using Service.Pattern;
using System.Collections.Generic;

namespace EM.Service
{
    public interface ICommentService : IService<Comment>
    {
        IEnumerable<Comment> SearchCommentsByName(string searchString);
    }
}
