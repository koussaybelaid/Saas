using EM.Data.Infrastructure;
using EM.Domain.Entities;
using Service.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EM.Service
{
    public class CommentService : Service<Comment>, ICommentService
    {
        static IDataBaseFactory Factory = new DataBaseFactory();//l'usine de fabrication du context
        static IUnitOfWork utk = new UnitOfWork(Factory);//unité de travail a besoin du factory pour communiquer avec la base
        public CommentService():base(utk)
        {

        }

        public IEnumerable<Comment> SearchCommentsByName(string searchString)
        {
            IEnumerable<Comment> CommentsDomain = GetMany();
            if (!String.IsNullOrEmpty(searchString))
            {
                CommentsDomain = GetMany(x => x.Text.Contains(searchString));
            }
            return CommentsDomain;
        }
    }
}
