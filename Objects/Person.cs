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
using System.Collections.ObjectModel;
using System.Xml.Linq;
using System;
using Vigo.Bas.ManagementAgent.Log;
using static FimSync_Ezma.Constants;

namespace FimSync_Ezma
{
    class Person : PersonAttributes
    {
        public Person(XElement person, Dictionary<string, Tuple<List<string>, string, string>> groups, KeyedCollection<string, ConfigParameter> configparameter, string company,
            List<ImportListItem> ImportedObjectsList, Dictionary<string, string> unitIdToUnitNumber, List<string> AllreadyImported, Dictionary<string, string> persIDHRM_userId,
            List<string> ignored_positionTypes)
        {
            new PersonAttributes();
            // PersonIDHRM. Anchor attribute

            List<String>_ignored_positionTypes = ignored_positionTypes;


            if (configparameter["Prefix personDN med selskapsnummer"].Value == "1")
            {
                PersonDN = company + "-" + person.Element("authentication").Element("userId").Value;
            }
            else
            {
                PersonDN = person.Element("authentication").Element("userId").Value;
            }
            PersonIdHRM = person.Attribute("personIdHRM").Value;
            Group_prefix = configparameter["Gruppeprefix"].Value;
            Group_suffix = configparameter["Gruppesuffix"].Value;

            // Initials
            try
            {
                Initials = person.Element("authentication").Element("initials").Value;
            }
            catch
            { }

            // userid 
            try
            {
                UserId = person.Element("authentication").Element("userId").Value;
            }
            catch
            { }

            // Firstname
            try
            {
                GivenName = person.Element("givenName").Value;
            }
            catch
            { }

            // Lastname
            try
            {
                FamilyName = person.Element("familyName").Value;
            }
            catch
            { }

            // Gendercode
            try
            {
                GenderCode = person.Element("genderCode").Value;
            }
            catch
            { }

            // SocialSecurityNumber
            try
            {
                SSN = person.Element("ssn").Value;
            }
            catch
            { }

            // Employeenumber in HRM
            try
            {
                EmployeeId = person.Element("employments").Element("employment").Element("employeeId").Value;
            }
            catch
            { }

            // Employment category
            try
            {
                CategoryId = person.Element("employments").Element("employment").Element("category").Attribute("id").Value;
            }
            catch
            { }

            // Postition description
            try
            {
                CategoryDescription = person.Element("employments").Element("employment").Element("category").Element("description").Value;
            }
            catch
            { }

            // Company information
            try
            {
                XElement _company = person.Element("employments").Element("employment").Element("company");
                OrganizationNumber = _company.Element("organizationNumber").Value;
                CompanyId = _company.Element("companyId").Value;
                CompanyName = _company.Element("companyName").Value;
            }
            catch
            { }

            // Birthdate
            try
            {
                DateOfBirth = person.Element("dateOfBirth").Value;
            }
            catch
            { }

            // Email address at work
            try
            {
                Email = person.Element("contactInfo").Element("email").Value;
            }
            catch
            { }

            // Private Email address
            try
            {
                PrivateEmail = person.Element("contactInfo").Element("privateEmail").Value;
            }
            catch
            { }

            // Work Mobile
            try
            {
                WorkMobile = person.Element("contactInfo").Element("mobilePhone").Value;
            }
            catch
            { }

            // Work Phone
            try
            {
                WorkPhone = person.Element("contactInfo").Element("workPhone").Value;
            }
            catch
            { }

            // Private Phone Number
            try
            {
                PrivatePhone = person.Element("contactInfo").Element("privatePhone").Value;
            }
            catch
            { }

            // Private Mobile Phone
            try
            {
                PrivateMobile = person.Element("contactInfo").Element("privateMobilePhone").Value;
            }
            catch { }

            // Title
            try
            {
                JobTitle = person.Element("careerInfo").Element("jobTitle").Value;
            }
            catch
            { }

            // Tasks
            try
            {
                WorkTasks = person.Element("careerInfo").Element("workTasks").Value;
            }
            catch
            { }

            // Start date
            try
            {
                StartDate = person.Element("employments").Element("employment").Element("startDate").Value;
            }
            catch
            { }
            
            // End date
            try
            {
                EndDate = person.Element("employments").Element("employment").Element("endDate").Value;
            }
            catch
            { }

            // Business Organization number
            try
            {
                BusinessNumberName = person.Element("employments").Element("employment").Element("businessNumbername").Value;
            }
            catch
            { }

            // EmployeePercentage independent to position
            try
            {
                EmploymentPercentage = person.Element("employments").Element("employment").Element("employmentPercentage").Value;
            }
            catch
            { }

            // Last changed timestamp 
            try
            {
                LastChangedDate = person.Element("lastChangedDate").Value;
            }
            catch
            { }

            // Add aliases to list
            try
            {
                foreach (XElement aliases in person.Element("authentication").Elements("alias"))
                {
                    try
                    {
                        Alias.Add(aliases.Value);
                    }
                    catch
                    { }
                }
            }
            catch
            { }

            // Adds hrmAuthentications to list (username defined in HRM against other connected systems)
            try
            {
                foreach (XElement hrmauths in person.Element("hrmAuthentications").Element("hrmAuthentication").Elements("loginId"))
                {
                    try
                    {
                        LoginId.Add(hrmauths.Value);
                    }
                    catch
                    { }
                }
            }
            catch
            { }


            // Parse each position
            try
            {
                foreach (XElement employments in person.Element("employments").Elements("employment"))
                {

                    int numOfPos = 0;
                    try
                    {
                        
                        foreach (XElement position in person.Element("employments").Element("employment").Element("positions").Elements("position"))
                        {
                            numOfPos++;
                        }

                        
                        //Loop throught all positions
                        foreach (XElement position in person.Element("employments").Element("employment").Element("positions").Elements("position"))
                        {
                            string _pos = position.Element("positionInfo").Element("positionType").Attribute("value").Value;
                            if (!_ignored_positionTypes.Contains(_pos) || String.IsNullOrWhiteSpace(_pos))
                                { 
                                    Employments emp = new Employments();
                                    try
                                    {
                                        try
                                        {
                                            isPrimaryPosition = Convert.ToBoolean(position.Attribute("isPrimaryPosition").Value);
                                        }
                                        catch
                                        { }

                                        //If primary position or if user has only one position, add attributes
                                        if (isPrimaryPosition == true || numOfPos == 1)
                                        {

                                            try
                                            {
                                                PositionPercentage = position.Element("positionPercentage").Value;
                                            }
                                            catch
                                            { }

                                            try
                                            {
                                                PositionStartDate = position.Element("positionStartDate").Value;
                                            }
                                            catch
                                            { }

                                            try
                                            {
                                                PositionEndDate = position.Element("positionEndDate").Value;
                                            }
                                            catch
                                            { }

                                            try
                                            {
                                                XElement _loc = position.Element("location");


                                                LocationName = _loc.Attribute("name").Value;
                                                LocationId = _loc.Attribute("value").Value;

                                            }
                                            catch
                                            { }

                                            try
                                            {
                                                YearlyHours = position.Element("yearlyHours").Value;
                                            }
                                            catch
                                            { }

                                            try
                                            {
                                                XElement _positionInfo = position.Element("positionInfo");
                                                XElement _positionCode = _positionInfo.Element("positionCode");
                                                XElement _positionType = _positionInfo.Element("positionType");
                                                XElement _publicPositionCode = _positionInfo.Element("publicPositionCode");
                                                XElement _positionStatistics = position.Element("positionStatistics");

                                                PositionCode = _positionCode.Attribute("positionCode").Value;
                                                PositionId = _positionCode.Attribute("positionId").Value;
                                                TableNr = _positionCode.Attribute("tableNr").Value;
                                                PositionCodeName = _positionCode.Attribute("name").Value;
                                                PositionTypeName = _positionType.Attribute("name").Value;
                                                PositionTypeValue = _positionType.Attribute("value").Value;
                                                PublicPositionCodeName = _publicPositionCode.Attribute("name").Value;
                                                PublicPositionCodeValue = _publicPositionCode.Attribute("value").Value;
                                                BusinessNumberName = _positionStatistics.Element("businessNumber").Attribute("name").Value;
                                                BusinessNumberValue = _positionStatistics.Element("businessNumber").Attribute("value").Value;
                                                CompanyNumberName = _positionStatistics.Element("businessNumber").Attribute("name").Value;
                                                CompanyNumberValue = _positionStatistics.Element("businessNumber").Attribute("value").Value;
                                            }
                                            catch
                                            { }

                                            // Add Unit and manager to strings
                                            try
                                            {
                                                XElement _chart = position.Element("chart");
                                                XElement _unit = _chart.Element("unit");

                                                //If Manager is present, add to strings
                                                try
                                                {
                                                    string managerpersId;
                                                    XElement _manager = _unit.Element("manager");
                                                    //if (configparameter["Prefix personDN med selskapsnummer"].Value == "1")
                                                    //{

                                                    managerpersId = company + "-" + _manager.Attribute("id").Value;
                                                    if (persIDHRM_userId.TryGetValue(managerpersId, out string persUserID))
                                                    {
                                                        if (configparameter["Prefix personDN med selskapsnummer"].Value == "1")
                                                        {
                                                            Manager = company + "-" + persUserID;
                                                        }
                                                        else
                                                        {
                                                            Manager = persUserID;
                                                        }
                                                    }

                                                    //Manager = company + "-" + _manager.Attribute("id").Value;
                                                    // }
                                                    //else
                                                    //{
                                                    //    managerpersId = _manager.Attribute("id").Value;
                                                    //    if (persIDHRM_userId.TryGetValue(managerpersId, out string persUserID))
                                                    //    {
                                                    //        Manager = persUserID;
                                                    //    }
                                                    //    Manager = _manager.Attribute("id").Value;
                                                    //}
                                                }
                                                catch { }

                                                ChartName = _chart.Attribute("name").Value;
                                                ChartId = _chart.Attribute("id").Value;
                                                UnitName = _unit.Attribute("name").Value;
                                                UnitId = _unit.Attribute("id").Value;
                                                unitIdToUnitNumber.TryGetValue(UnitId, out string strvalue);
                                                UnitIdRef = strvalue;
                                            }
                                            catch
                                            { }

                                            // Add dimensions to string
                                            try
                                            {
                                                XElement _cost = position.Element("costCentres");
                                                XElement _dim2 = _cost.Element("dimension2");
                                                XElement _dim3 = _cost.Element("dimension3");

                                                Dimension2Name = _dim2.Attribute("name").Value;
                                                Dimension2Value = _dim2.Attribute("value").Value;
                                                Dimension3Name = _dim3.Attribute("name").Value;
                                                Dimension3Value = _dim3.Attribute("value").Value;
                                            }
                                            catch
                                            { }
                                        }

                                    // Adds info about other positions to list if user has more than one position
                                    // if (isPrimaryPosition == false && numOfPos >= 2)
                                    if (isPrimaryPosition == false)
                                        {
                                            try
                                            {
                                                XElement _chart = position.Element("chart");
                                                XElement _unit = _chart.Element("unit");
                                                XElement _manager = _unit.Element("manager");

                                                SecondManager = new List<string>();
                                                SecondChartName = new List<string>();
                                                SecondChartId = new List<string>();
                                                SecondUnitName = new List<string>();
                                                SecondUnitId = new List<string>();

                                                if (persIDHRM_userId.TryGetValue(company + "-" + _manager.Attribute("id").Value, out string persUserID))
                                                {
                                                    if (configparameter["Prefix personDN med selskapsnummer"].Value == "1")
                                                    {
                                                        SecondManager.Add(company + "-" + persUserID);
                                                    }
                                                    else
                                                    {
                                                        SecondManager.Add(persUserID);
                                                    }
                                                }

                                                //SecondManager.Add(_manager.Attribute("id").Value);
                                                SecondChartName.Add(_chart.Attribute("name").Value);
                                                SecondChartId.Add(_chart.Attribute("id").Value);
                                                SecondUnitName.Add(_unit.Attribute("name").Value);
                                                SecondUnitId.Add(_unit.Attribute("id").Value);

                                                XElement _positionInfo = position.Element("positionInfo");
                                                XElement _positionCode = _positionInfo.Element("positionCode");
                                                XElement _positionType = _positionInfo.Element("positionType");
                                                XElement _publicPositionCode = _positionInfo.Element("publicPositionCode");
                                                XElement _positionStatistics = position.Element("positionStatistics");

                                                if (String.IsNullOrWhiteSpace(PositionCode))
                                                {
                                                    PositionCode = _positionCode.Attribute("positionCode").Value;
                                                }
                                                if (String.IsNullOrWhiteSpace(PositionId))
                                                { 
                                                    PositionId = _positionCode.Attribute("positionId").Value;
                                                }
                                                if (String.IsNullOrWhiteSpace(TableNr))
                                                {
                                                    TableNr= _positionCode.Attribute("tableNr").Value;
                                                }
                                                if (String.IsNullOrWhiteSpace(PositionCodeName))
                                                {
                                                    PositionCodeName = _positionCode.Attribute("name").Value;
                                                }
                                                if (String.IsNullOrWhiteSpace(PositionTypeName))
                                                {
                                                    PositionTypeName = _positionType.Attribute("name").Value;
                                                }
                                                if (String.IsNullOrWhiteSpace(PositionTypeValue))
                                                {
                                                    PositionTypeValue = _positionType.Attribute("value").Value;
                                                }
                                                if (String.IsNullOrWhiteSpace(PublicPositionCodeName))
                                                {
                                                    PublicPositionCodeName = _publicPositionCode.Attribute("name").Value;
                                                }
                                                if (String.IsNullOrWhiteSpace(PublicPositionCodeValue))
                                                {
                                                    PublicPositionCodeValue = _publicPositionCode.Attribute("value").Value;
                                                }
                                                try
                                                {
                                                    XElement _loc = position.Element("location");


                                                    LocationName = _loc.Attribute("name").Value;
                                                    LocationId = _loc.Attribute("value").Value;

                                                }
                                                catch
                                                { }



                                        }
                                            catch
                                            { }
                                        }
                                    }
                                    catch
                                    { }

                                    try
                                    {
                                        XElement _chart = position.Element("chart");
                                        XElement _unit = _chart.Element("unit");
                                        GrpName = _unit.Attribute("name").Value;
                                        GrpId = _unit.Attribute("id").Value;

                                        // Add Orgunitid and description to employments
                                        emp.OrgUnitId = GrpId;
                                        emp.OrgUnitDescription = GrpName;
                                    }
                                    catch
                                    { }

                                    // If create unit groups - Create unit groups and save to groups dictionary
                                    if (configparameter["Unit grupper"].Value == "1")
                                    {
                                        try
                                        {
                                            XElement _chart = position.Element("chart");
                                            XElement _unit = _chart.Element("unit");
                                            GrpName = _unit.Attribute("name").Value;
                                            GrpId = _unit.Attribute("id").Value;

                                            // Add Orgunitid and description to employments
                                            //emp.OrgUnitId = GrpId;
                                            //emp.OrgUnitDescription = GrpName;

                                            if (!groups.ContainsKey(GrpId))
                                            {
                                                groups.Add(GrpId, Tuple.Create(new List<string>(), GrpName, "department"));
                                                if (!groups[GrpId].Item1.Contains(PersonDN))
                                                {
                                                    groups[GrpId].Item1.Add(PersonDN);
                                                }
                                            }
                                            else
                                            {
                                                if (!groups[GrpId].Item1.Contains(PersonDN))
                                                {
                                                    groups[GrpId].Item1.Add(PersonDN);
                                                }
                                            }
                                        }
                                        catch
                                        { }
                                    }


                                    // If create businessNumber groups - Create businessNumber groups and save to groups dictionary
                                    if (configparameter["BusinessNumber grupper"].Value == "1")
                                    {
                                        try
                                        {
                                            XElement _positionStatistics = position.Element("positionStatistics");
                                            XElement _businessNumber = _positionStatistics.Element("businessNumber");
                                            GrpName = _businessNumber.Attribute("name").Value;
                                            GrpId = _businessNumber.Attribute("value").Value;


                                            if (!groups.ContainsKey(GrpId))
                                            {
                                                groups.Add(GrpId, Tuple.Create(new List<string>(), GrpName, "orgUnit"));
                                                if (!groups[GrpId].Item1.Contains(PersonDN))
                                                {
                                                    groups[GrpId].Item1.Add(PersonDN);
                                                }
                                            }
                                            else
                                            {
                                                if (!groups[GrpId].Item1.Contains(PersonDN))
                                                {
                                                    groups[GrpId].Item1.Add(PersonDN);
                                                }
                                            }
                                        }
                                        catch
                                        { }
                                    }


                                    // If create company groups - Create company groups and save to groups dictionary
                                    if (configparameter["Company grupper"].Value == "1")
                                    {

                                        try
                                        {
                                            XElement _positionStatistics = position.Element("positionStatistics");
                                            XElement _businessNumber = _positionStatistics.Element("companyNumber");
                                            GrpName = _businessNumber.Attribute("name").Value;
                                            GrpId = _businessNumber.Attribute("value").Value;


                                            if (!groups.ContainsKey(GrpId))
                                            {
                                                groups.Add(GrpId, Tuple.Create(new List<string>(), GrpName, "company"));
                                                if (!groups[GrpId].Item1.Contains(PersonDN))
                                                {
                                                    groups[GrpId].Item1.Add(PersonDN);
                                                }
                                            }
                                            else
                                            {
                                                if (!groups[GrpId].Item1.Contains(PersonDN))
                                                {
                                                    groups[GrpId].Item1.Add(PersonDN);
                                                }
                                            }
                                        }
                                        catch
                                        { }
                                    }

                                    // If create dimension groups - Create dimension groups and save to groups dictionary
                                    if (configparameter["Dimension grupper"].Value == "1")
                                    {
                                        try
                                        {
                                            XElement _costCentres = position.Element("costCentres");
                                            XElement _dimension2 = _costCentres.Element("dimension2");
                                            GrpName = _dimension2.Attribute("name").Value;
                                            GrpId = _dimension2.Attribute("value").Value;

                                            if (!groups.ContainsKey(GrpId))
                                            {
                                                groups.Add(GrpId, Tuple.Create(new List<string>(), GrpName, "dimension2"));
                                                if (!groups[GrpId].Item1.Contains(PersonDN))
                                                {
                                                    groups[GrpId].Item1.Add(PersonDN);
                                                }
                                            }
                                            else
                                            {
                                                if (!groups[GrpId].Item1.Contains(PersonDN))
                                                {
                                                    groups[GrpId].Item1.Add(PersonDN);
                                                }
                                            }
                                        }
                                        catch
                                        { }
                                    }

                                    // Adds positions to Employments
                                    try
                                    {
                                        emp.MainPosition = Convert.ToBoolean(position.Attribute("isPrimaryPosition").Value);
                                    }
                                    catch
                                    { }

                                    try
                                    {
                                        emp.E_EmployeeId = employments.Element("employeeId").Value;
                                        emp.E_HRMID = PersonDN;
                                    }
                                    catch
                                    { }

                                    try
                                    {
                                        string pos = null, unid = null;
                                        try
                                        {
                                            pos = position.Element("positionInfo").Element("positionCode").Attribute("positionId").Value;
                                        }
                                        catch { }

                                        try
                                        {
                                            unid = position.Element("chart").Element("unit").Attribute("id").Value;
                                        }
                                        catch { }


                                        if (!string.IsNullOrEmpty(unid) && !string.IsNullOrEmpty(pos))
                                        {
                                            emp.EmploymentId = PersonDN + "-" + unid + "-" + pos;
                                            // Adds employmentID to array. To use when existing users have positions from get-future persons
                                            if (!_myEmployments.Contains(emp.EmploymentId))
                                            {
                                                _myEmployments.Add(emp.EmploymentId);
                                            }
                                        }
                                        else
                                        {
                                            if (configparameter["Logg med debug?"].Value == "1")
                                            {
                                                Logger.Log.DebugFormat("PositionId eller unit id mangler på ressursnummer {0}", EmployeeId);
                                            }
                                        }
                                    }
                                    catch
                                    { }

                                    try
                                    {
                                        emp.E_ManagerId = position.Element("chart").Element("unit").Element("manager").Attribute("id").Value;
                                    }
                                    catch { }

                                    try
                                    {
                                        emp.PositionPercentage = position.Element("positionPercentage").Value;
                                    }
                                    catch
                                    { }

                                    try
                                    {
                                        emp.DateFrom = position.Element("positionStartDate").Value;
                                    }
                                    catch
                                    { }

                                    try
                                    {
                                        emp.DateTo = position.Element("positionEndDate").Value;
                                    }
                                    catch
                                    { }

                                    try
                                    {
                                        emp.DateTo = position.Element("positionEndDate").Value;
                                    }
                                    catch
                                    { }

                                    try
                                    {
                                        XElement _positionInfo = position.Element("positionInfo");
                                        XElement _positionCode = _positionInfo.Element("positionCode");
                                        XElement _positionType = _positionInfo.Element("positionType");
                                        XElement _publicPositionCode = _positionInfo.Element("publicPositionCode");

                                        //Add positionId and description to employments array
                                        emp.PositionId = _positionCode.Attribute("positionId").Value;
                                        emp.PositionCodeName = _positionCode.Attribute("name").Value;
                                        emp.PositionCodeValue = _positionCode.Attribute("positionCode").Value;
                                        emp.PositionTypeName = _positionType.Attribute("name").Value;
                                        emp.PositionTypeValue = _positionType.Attribute("value").Value;
                                        emp.PositionDescription = _publicPositionCode.Attribute("name").Value;
                                        emp.PositionType = _positionType.Attribute("value").Value;
                                    }
                                    catch
                                    { }

                                    if (!string.IsNullOrEmpty(emp.EmploymentId))
                                    {
                                        if (!AllreadyImported.Contains(emp.EmploymentId))
                                        {
                                            AllreadyImported.Add(emp.EmploymentId);
                                            ImportedObjectsList.Add(new ImportListItem() { employments = emp });
                                            if (configparameter["Logg med debug?"].Value == "1")
                                            {
                                                Logger.Log.DebugFormat("La til employment {0} for ressursnummer {1}", emp.EmploymentId, emp.E_EmployeeId);
                                            }
                                        }
                                    }
                            }
                        }
                    }
                    catch
                    {
                        if (configparameter["Logg med debug?"].Value == "1")
                        {
                            Logger.Log.DebugFormat("Fant ingen aktive stillinger for ressursnummer {0}", EmployeeId);
                        }
                    }

                }
                Employments = _myEmployments;
            }
            catch
            {
                
            }
        }
        
      
        internal Microsoft.MetadirectoryServices.CSEntryChange GetCSEntryChange()
        {
            // Add all to CSEntry
            CSEntryChange csentry = CSEntryChange.Create();
            csentry.ObjectModificationType = ObjectModificationType.Add;
            csentry.ObjectType = SchemaTypes.person;

            // personIdHRM must always be present
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.personDN, PersonDN));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.personIdHRM, PersonIdHRM));

            // Add to csentry if values is present 

            if (GivenName != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.givenName, GivenName));

            if (FamilyName != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.familyName, FamilyName));

            if (SSN != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.ssn, SSN));

            if (EmployeeId != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.employeeId, EmployeeId));

            if (UserId != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.userId, UserId));

            if (Initials != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.initials, Initials));

            if (JobTitle != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.jobTitle, JobTitle));

            if (WorkTasks != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.workTasks, WorkTasks));

            if (DateOfBirth != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.dateOfBirth, DateOfBirth));

            if (WorkMobile != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.workMobile, WorkMobile));

            if (WorkPhone != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.workPhone, WorkPhone));

            if (PrivateMobile != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.privateMobile, PrivateMobile));

            if (PrivatePhone != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.privatePhone, PrivatePhone));

            if (GenderCode != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.genderCode, GenderCode));

            if (Email != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.email, Email));

            if (PrivateEmail != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.privateEmail, PrivateEmail));

            if (StartDate != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.startDate, StartDate));

            if (EndDate != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.endDate, EndDate));

            if (EmploymentPercentage != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.employmentPercentage, EmploymentPercentage));

            if (LocationName != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.locationName, LocationName));
            
            if (LocationId != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.locationId, LocationId));

            if (LastChangedDate != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.lastChangedDate, LastChangedDate));

            if (CategoryId != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.category_id, CategoryId));

            if (CategoryDescription != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.categorydescription, CategoryDescription));

            if (PositionPercentage != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.positionPercentage, PositionPercentage));

            if (PositionStartDate != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.positionStartDate, PositionStartDate));

            if (PositionEndDate != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.positionEndDate, PositionEndDate));

            if (PositionCode != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.positionCode, PositionCode));

            if (PositionId != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.positionId, PositionId));

            if (TableNr != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.tableNr, TableNr));

            if (PositionCodeName != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.positionCodeName, PositionCodeName));

            if (PositionTypeValue != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.positionTypeValue, PositionTypeValue));

            if (PositionTypeName != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.positionTypeName, PositionTypeName));

            if (PublicPositionCodeName != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.publicPositionCodeName, PublicPositionCodeName));

            if (PublicPositionCodeValue != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.publicPositionCodeValue, PublicPositionCodeValue));

            if (Dimension2Name != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.dimension2name, Dimension2Name));

            if (Dimension2Value != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.dimension2value, Dimension2Value));

            if (Dimension3Name != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.dimension3name, Dimension3Name));

            if (Dimension3Value != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.dimension3value, Dimension3Value));

            if (Manager != null)
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.manager, Manager));
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.personManageridAsString, Manager));
            }

            if (UnitId != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.unitId, UnitId));
                
            if (UnitIdRef != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.unitIdRef, UnitIdRef));

            if (UnitName != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.unitName, UnitName));

            if (ChartId != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.chartId, ChartId));

            if (ChartName != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.chartName, ChartName));

            if (CompanyId != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.companyId, CompanyId));

            if (CompanyName != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.companyName, CompanyName));

            if (OrganizationNumber != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.organizationNumber, OrganizationNumber));

            if (BusinessNumberName != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.businessNumbername, BusinessNumberName));

            if (BusinessNumberValue != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.businessNumbervalue, BusinessNumberValue));

            if (CompanyNumberName != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.companyNumbername, CompanyNumberName));

            if (CompanyNumberValue != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.companyNumbervalue, CompanyNumberValue));


            // Add to multivalue alias
            if (Alias.Count >= 1)
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.alias, Alias));
            }

            // Add to multivalue loginID
            if (LoginId.Count >= 1)
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.loginID, LoginId));
            }

            // Add multivalues

            if (Employments != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.employments, Employments));
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.employmentsAsString, Employments));
            



            if (SecondManager != null)
                foreach (string sec in SecondManager)
                {
                    csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.secondmanager, sec));
                }

            if (SecondChartName != null)
                foreach (string sec in SecondChartName)
                {
                    csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.secondchartName, sec));
                }

            if (SecondChartId != null)
                foreach (string sec in SecondChartId)
                {
                    csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.secondchartId, sec));
                }

            if (SecondUnitName != null)
                foreach (string sec in SecondUnitName)
                {
                    csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.secondunitName, sec));
                }

            if (SecondUnitId != null)
                foreach (string sec in SecondUnitId)
                {
                    csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.secondunitId, sec));
                }
            
            // Return
            return csentry;

        }
    }
}
