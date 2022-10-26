using backend.Commons;
// using GreenHouseControlWebApp.Entities.IdEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Identity;


namespace backend.Model.AppEntity
{
    public class LandOwnerShip:Auditable
    {
        public int id {get;set;}
        public int UserId {get;set;}
        public ApplicationUser User {get;set;}
        public int LandId {get;set;}
        public Land Land {get;set;}
    }
}
