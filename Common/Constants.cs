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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FimSync_Ezma
{
    class Constants
    {
        public const string dateFormat = "yyyy-MM-dd";

        public struct SchemaAttributes
        {
            #region Person
            public const string personDN = "personDN";
            public const string personIdHRM = "personIdHRM";
            public const string givenName = "givenName";
            public const string familyName = "familyName";
            public const string ssn = "ssn";
            public const string dateOfBirth = "dateOfBirth";
            public const string genderCode = "genderCode";
            public const string alias = "alias";
            public const string loginID = "loginID";
            public const string initials = "initials";
            public const string employeeId = "employeeId";
            public const string userIdalias = "userIdalias";
            public const string userId = "userId";
            public const string manager = "manager";
            public const string personManageridAsString = "personManageridAsString";
            public const string workTasks = "workTasks";
            public const string jobTitle = "jobTitle";
            public const string startDate = "startDate";
            public const string endDate = "endDate";
            public const string email = "email";
            public const string privateEmail = "privateEmail";
            public const string workMobile = "workMobile";
            public const string workPhone = "workPhone";
            public const string privatePhone = "privatePhone";
            public const string privateMobile = "privateMobile";
            public const string category_id = "category id";
            public const string categorydescription = "categorydescription";
            public const string lastChangedDate = "lastChangedDate";
            public const string employmentPercentage = "employmentPercentage";
            public const string businessNumbername = "businessNumbername";
            public const string businessNumbervalue = "businessNumbervalue";
            public const string companyId = "companyId";
            public const string companyName = "companyName";
            public const string organizationNumber = "organizationNumber";
            public const string companyNumbername = "companyNumbername";
            public const string companyNumbervalue = "companyNumbervalue";
            public const string positionPercentage = "positionPercentage";
            public const string positionStartDate = "positionStartDate";
            public const string positionEndDate = "positionEndDate";
            public const string locationName = "locationName";
            public const string locationId = "locationId";
            public const string yearlyHours = "yearlyHours";
            public const string positionCode = "positionCode";
            public const string positionId = "positionId";
            public const string tableNr = "tableNr";
            public const string positionCodeName = "positionCodeName";
            public const string positionTypeValue = "positionTypeValue";
            public const string positionTypeName = "positionTypeName";
            public const string publicPositionCodeName = "publicPositionCodeName";
            public const string publicPositionCodeValue = "publicPositionCodeValue";
            public const string dimension2name = "dimension2name";
            public const string dimension2value = "dimension2value";
            public const string dimension3name = "dimension3name";
            public const string dimension3value = "dimension3value";
            public const string employments = "employments";
            public const string employmentsAsString = "employmentsAsString";

            public const string chartId = "chartId";
            public const string chartName = "chartName";
            public const string unitId = "unitId";
            public const string unitIdRef = "unitIdRef";
            public const string unitName = "unitName";

            public const string secondmanager = "secondmanager";
            public const string secondchartName = "secondchartName";
            public const string secondchartId = "secondchartId";
            public const string secondunitName = "secondunitName";
            public const string secondunitId = "secondunitId";
            #endregion
            
            #region Group
            public const string groupIdDN = "groupId";
            public const string groupName = "groupName";
            public const string groupDescription = "groupDescription";
            public const string groupSourceId = "groupSourceId";
            public const string members = "members";
            public const string groupType = "groupType";
            #endregion

            #region Unit
            public const string unitorgkodeDN = "unitorgkode";

            public const string unitname = "unitname";
            public const string employee_id = "employee Id";
            public const string unitcompany = "unitcompany";
            public const string unitid = "unitid";
            public const string unitparent = "unitparent";
            public const string unitparentAsString = "unitparentAsString";
            public const string managerid = "managerid";
            public const string manageridAsString = "manageridAsString";
            public const string managername = "managername";
            public const string phone = "phone";
            public const string fax = "fax";
            public const string postaladdress1 = "postaladdress1";
            public const string postaladdress2 = "postaladdress2";
            public const string postalcode = "postalcode";
            public const string postalarea = "postalarea";
            public const string visitaddress1 = "visitaddress1";
            public const string visitaddress2 = "visitaddress2";
            public const string visitpostalcode = "visitpostalcode";
            public const string visitpostalarea = "visitpostalarea";
            public const string unitemail = "unitemail";
            #endregion

            #region Employment
            public const string EmploymentIdDN = "EmploymentId";

            public const string E_EmployeeId = "E_EmployeeId";
            public const string E_HRMID = "E_HRMID";
            public const string DateFrom = "DateFrom";
            public const string DateTo = "DateTo";
            public const string OrgUnitId = "OrgUnitId";
            public const string OrgUnitDescription = "OrgUnitDescription";
            public const string PostCode = "PostCode";
            public const string PostCodeDescription = "PostCodeDescription";
            public const string MainPosition = "MainPosition";
            public const string E_ManagerId = "E_ManagerId";
            public const string PositionTitle = "PositionTitle";
            public const string PositionId = "PositionId";
            public const string PositionType = "PositionType";
            public const string PositionPercentage = "PositionPercentage";
            public const string PositionDescription = "PositionDescription";
            public const string E_positionTypeValue = "E_positionTypeValue";
            public const string E_positionTypeName = "E_positionTypeName";
            public const string E_positionCodeName = "E_positionCodeName";
            public const string E_positionCodeValue = "E_positionCodeValue";
            

            #endregion
        }

        public struct SchemaTypes
        {
            public const string person = "person";
            public const string group = "group";
            public const string unit = "unit";
            public const string employments = "employments";
        }

        public struct Method
        {
            public const string GET = "GET";
            public const string PUT = "PUT";
            public const string POST = "POST";
            public const string DELETE = "DELETE";
        }
    }
}
