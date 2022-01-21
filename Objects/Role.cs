// VIGOBAS Identity Management System 
//  Copyright (C) 2021  Vigo IKS 
//  
//  Documentation - visit https://vigobas.vigoiks.no/ 
//  
//  This program is free software: you can redistribute it and/or modify 
//  it under the terms of the GNU Affero General Public License as 
//  published by the Free Software Foundation, either version 3 of the 
//  License, or (at your option) any later version. 
//  
//  This program is distributed in the hope that it will be useful, 
//  but WITHOUT ANY WARRANTY, without even the implied warranty of 
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the 
//  GNU Affero General Public License for more details. 
//  
//  You should have received a copy of the GNU Affero General Public License 
//  along with this program.  If not, see https://www.gnu.org/licenses/.

using Microsoft.MetadirectoryServices;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

namespace FimSync_Ezma
{
    class Role
    {
        // Definisjoner for bruk i koden
        string roleid;
        Dictionary<string, List<string>> _roles = new Dictionary<string, List<string>>();
        Dictionary<string, string> _rolenames = new Dictionary<string, string>();

        public Role(string rolekey, List<string> rolevalues, Dictionary<string, string> rolenames)
        {
            _roles.Add(rolekey, rolevalues);
            _rolenames = rolenames;
            roleid = rolekey;
        }

        public string Anchor()
        {
            return roleid;
        }

        public override string ToString()
        {
            return roleid;
        }

        //internal Microsoft.MetadirectoryServices.CSEntryChange GetCSEntryChange()
        //{
        //    CSEntryChange csentry = CSEntryChange.Create();
        //    csentry.ObjectModificationType = ObjectModificationType.Add;
        //    csentry.ObjectType = "role";
            
        //    foreach (var role in _roles)
        //    {
        //        csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("roleid", role.Key));
        //        csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("role description", _rolenames[role.Key].ToString()));
                
        //        IList<object> emp = new List<object>();
        //        foreach (var employee in role.Value)
        //        {
        //            emp.Add(employee.ToString());
        //        }
        //        csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("employee Id", emp));
        //    }
        //    return csentry;

        //}
    }
}
