﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VDCompany.Models.Objects;

namespace VDCompany.Models.DTO
{
    public class MyCaseDTO
    {
        public Case Case = new Case();
        public User User = new User();
        public Admin Admin = new Admin();
        public Lawyer Lawyer = new Lawyer();


        public MyCaseDTO(Case @case, User user)
        {
            Case = @case;
            User = user;
        }
        public MyCaseDTO(Case @case, Admin user)
        {
            Case = @case;
            Admin = user;
        }
        public MyCaseDTO(Case @case, Lawyer user)
        {
            Case = @case;
            Lawyer = user;
        }
    }
}
