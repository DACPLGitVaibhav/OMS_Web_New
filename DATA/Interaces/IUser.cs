using DATA.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DATA.Interfaces
{
   public interface IUser
    {
        Users getbyid(int id);
        Users getbyName(string name);
        List<Users> Getall();
        void Add(Users users);
        void Edit(Users users);
        void Delete(Users users);
    }
}
