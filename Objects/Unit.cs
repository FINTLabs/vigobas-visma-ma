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
using System.Xml.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using static FimSync_Ezma.Constants;

namespace FimSync_Ezma
{
    class Unit : UnitAttributes
    {
        public Unit(XElement unit, Dictionary<string, string> unitIdToUnitNumber, KeyedCollection<string, ConfigParameter> configparameter, string company, Dictionary<string, string> persIDHRM_userId)
        {

            UnitId = unit.Attribute("id").Value.Trim();

            try { UnitCompany = unit.Attribute("company").Value; }
            catch { }

            try { UnitOrgCode = "unit" + unit.Attribute("kode").Value; }
            catch { } 

            try { unitIdToUnitNumber.Add(UnitId, UnitOrgCode); }
            catch { }

            try { UnitName = unit.Attribute("name").Value; }
            catch { }

            try { UnitParent = "unit" + unit.Attribute("parentid").Value; }
            catch { }

            try
            {
                if (persIDHRM_userId.TryGetValue(company + "-" + unit.Element("manager").Attribute("id").Value, out string managerid))
                {
                    if (configparameter["Prefix personDN med selskapsnummer"].Value == "1")
                    {
                        ManagerId = company + "-" + managerid;
                    }
                    else
                    {
                        ManagerId = managerid;
                    }
                }
            }
            catch { }

            try { ManagerName = unit.Element("manager").Attribute("name").Value; }
            catch { }
            
            try { Phone = unit.Element("contactInfo").Element("phone").Value; }
            catch { }

            try { Fax = unit.Element("contactInfo").Element("fax").Value; }
            catch { }

            try { PostalAddress1 = unit.Element("contactInfo").Element("postaladdress1").Value; }
            catch { }

            try { PostalAddress2 = unit.Element("contactInfo").Element("postaladdress2").Value; }
            catch { }

            try { PostalCode = unit.Element("contactInfo").Element("postalcode").Value; }
            catch { }

            try { PostalArea = unit.Element("contactInfo").Element("postalarea").Value; }
            catch { }

            try { VisitAddress1 = unit.Element("contactInfo").Element("visitaddress1").Value; }
            catch { }

            try { VisitAddress2 = unit.Element("contactInfo").Element("visitaddress2").Value; }
            catch { }

            try { VisitPostalCode = unit.Element("contactInfo").Element("visitpostalcode").Value; }
            catch { }

            try { VisitPostalArea = unit.Element("contactInfo").Element("visitpostalarea").Value; }
            catch { }

            try { Email = unit.Element("contactInfo").Element("email").Value; }
            catch { }

            //foreach (XElement task in unit.Element("tasks").Elements("task"))
            //{
            //    try
            //    {
            //        if (!tasks.ContainsKey(task.Attribute("id").Value))
            //        {
                        
            //            tasks.Add(task.Attribute("id").Value, new List<string>());
            //            tasknames.Add(task.Attribute("id").Value, task.Attribute("description").Value);

            //            foreach (XElement employee in task.Element("employees").Elements("employee"))
            //            {
            //                if (!tasks[task.Attribute("id").Value].Contains(employee.Attribute("id").Value))
            //                {
            //                    tasks[task.Attribute("id").Value].Add(employee.Attribute("id").Value);
            //                }
            //            }
            //        }
            //        else
            //        {
            //            foreach (XElement employee in task.Elements("task"))
            //            {
            //                if (!tasks[unitid + "-" + task.Attribute("id").Value].Contains(employee.Attribute("id").Value))
            //                {
            //                    tasks[unitid + "-" + task.Attribute("id").Value].Add(employee.Attribute("id").Value);
            //                }
            //            }
            //        }
            //    }
            //    catch { }
            //}

            //foreach (XElement role in unit.Elements("roles"))
            //{
            //    try
            //    {
            //        if (!roles.ContainsKey(unitid + "-" + role.Attribute("id").Value))
            //        {
            //            roles.Add(unitid + "-" + role.Attribute("id").Value, new List<string>());
            //            rolenames.Add(unitid + "-" + role.Attribute("id").Value, role.Element("description").Value);
            //            foreach (XElement employee in role.Elements("role"))
            //            {
            //                if (!roles[unitid + "-" + role.Attribute("id").Value].Contains(employee.Attribute("id").Value))
            //                {
            //                    roles[unitid + "-" + role.Attribute("id").Value].Add(employee.Attribute("id").Value);
            //                }
            //            }
            //        }
            //        else
            //        {
            //            foreach (XElement employee in role.Elements("role"))
            //            {
            //                if (!roles[unitid + "-" + role.Attribute("id").Value].Contains(employee.Attribute("id").Value))
            //                {
            //                    roles[unitid + "-" + role.Attribute("id").Value].Add(employee.Attribute("id").Value);
            //                }
            //            }
            //        }
            //    }
            //    catch { }
            //}
            
        }
        
        public string Anchor()
        {
            return UnitId;
        }

        public override string ToString()
        {
            return UnitId;
        }

        internal Microsoft.MetadirectoryServices.CSEntryChange GetCSEntryChange()
        {
            CSEntryChange csentry = CSEntryChange.Create();
            csentry.ObjectModificationType = ObjectModificationType.Add;
            csentry.ObjectType = SchemaTypes.unit;


            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.unitorgkodeDN, UnitOrgCode));


            if (!string.IsNullOrWhiteSpace(UnitName))
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.unitname, UnitName));

            if (!string.IsNullOrWhiteSpace(UnitId))
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.unitid, UnitId));

            if (!string.IsNullOrWhiteSpace(UnitParent))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.unitparent, UnitParent));
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.unitparentAsString, UnitParent));
            }

            if (!string.IsNullOrWhiteSpace(ManagerId))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.managerid, ManagerId));
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.manageridAsString, ManagerId));
            }

            if (!string.IsNullOrWhiteSpace(ManagerName))
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.managername, ManagerName));

            if (!string.IsNullOrWhiteSpace(Phone))
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.phone, Phone));

            if (!string.IsNullOrWhiteSpace(Fax))
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.fax, Fax));

            if (!string.IsNullOrWhiteSpace(PostalAddress1))
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.postaladdress1, PostalAddress1));

            if (!string.IsNullOrWhiteSpace(PostalAddress2))
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.postaladdress2, PostalAddress2));

            if (!string.IsNullOrWhiteSpace(PostalCode))
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.postalcode, PostalCode));

            if (!string.IsNullOrWhiteSpace(PostalArea))
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.postalarea, PostalArea));

            if (!string.IsNullOrWhiteSpace(VisitAddress1))
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.visitaddress1, VisitAddress1));

            if (!string.IsNullOrWhiteSpace(VisitAddress2))
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.visitaddress2, VisitAddress2));

            if (!string.IsNullOrWhiteSpace(VisitPostalCode))
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.visitpostalcode, VisitPostalCode));

            if (!string.IsNullOrWhiteSpace(VisitPostalArea))
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.visitpostalarea, VisitPostalArea));

            if (!string.IsNullOrWhiteSpace(Email))
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.unitemail, Email));

           
            //foreach (var task in tasks)
            //{
            //    string key = task.Key;
            //    string header = unitid;
            //    List<string> content = new List<string>();


            //    csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.taskid", key));
            //    csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.task description", tasknames[task.Key].ToString()));
                
            //    IList<object> emp = new List<object>();
            //    foreach (var employee in task.Value)
            //    {
            //        emp.Add(employee.ToString());
            //    }
            //    csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.employee Id", emp));
            //}

            //foreach (var role in roles)
            //{
                
            //        csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.roleid", role.Key));
            //        csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.role description", rolenames[role.Key].ToString()));
                
            //    IList<object> emp = new List<object>();
            //    foreach (var employee in role.Value)
            //    {
            //        emp.Add(employee.ToString());
            //    }
            //    csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.employee Id", emp));
            //}
            return csentry;

        }
    }
}
