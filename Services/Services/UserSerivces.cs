using DATA;
using DATA.Interfaces;
using DATA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services.Services
{
   public class UserSerivces:IUser
    {
        private readonly ContextClass _context;

        public UserSerivces(ContextClass context)
        {
            _context = context;
        }

        public void Add(Users users)
        {
            _context.users.Add(users);
            _context.SaveChanges();
        }

        public void Delete(Users users)
        {
            var data = _context.users.Find(users.Id);
            _context.users.Remove(data);
            _context.SaveChanges();
        }

        public void Edit(Users users)
        {
            _context.users.Update(users);
            _context.SaveChanges();
        }

        public List<Users> Getall()
        {
            return _context.users.ToList();
        }

        public Users getbyid(int id)
        {
            return _context.users.Where(x => x.Id == id).FirstOrDefault();
        }
        public Users getbyName(string name)
        {
            return _context.users.Where(x => x.UserName == name && x.Isactive==true).FirstOrDefault();
        }
    }
}
