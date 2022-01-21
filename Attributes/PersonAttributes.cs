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

using System.Collections.Generic;

namespace FimSync_Ezma
{
    public class PersonAttributes
    {
        public string PersonIdHRM { get; set; }
        public string PersonDN { get; set; }
        public string UserId { get; set; }
        public string Initials { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string SSN { get; set; }
        public string EmployeeId { get; set; }
        public string WorkMobile { get; set; }
        public string PrivateMobile { get; set; }
        public string PrivateEmail { get; set; }
        public string Email { get; set; }
        public string PrivatePhone { get; set; }
        public string LastChangedDate { get; set; }
        public string WorkPhone { get; set; }
        public string DateOfBirth { get; set; }
        public string WorkTasks { get; set; }
        public string JobTitle { get; set; }
        public string GenderCode { get; set; }
        public string CategoryDescription { get; set; }
        public string CategoryId { get; set; }
        public string OrganizationNumber { get; set; }
        public string CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string EmploymentPercentage { get; set; }
        public string PositionPercentage { get; set; }
        public string PositionStartDate { get; set; }
        public string PositionEndDate { get; set; }
        public string LocationName { get; set; }
        public string LocationId { get; set; }
        public string BusinessNumberValue { get; set; }
        public string BusinessNumberName { get; set; }
        public string CompanyNumberName { get; set; }
        public string CompanyNumberValue { get; set; }
        public string YearlyHours { get; set; }
        public string PositionCode { get; set; }
        public string PositionId { get; set; }
        public string TableNr { get; set; }
        public string PositionCodeName { get; set; }
        public string PositionTypeValue { get; set; }
        public string PositionTypeName { get; set; }
        public string PublicPositionCodeName { get; set; }
        public string PublicPositionCodeValue { get; set; }
        public string Manager { get; set; }
        public string Dimension2Name { get; set; }
        public string Dimension3Name { get; set; }
        public string Dimension2Value { get; set; }
        public string Dimension3Value { get; set; }
        public string ChartId { get; set; }
        public string ChartName { get; set; }
        public string UnitId { get; set; }
        public string UnitName { get; set; }
        public string UnitIdRef { get; set; }
        public string GrpName { get; set; }
        public string GrpId { get; set; }
        public string Group_prefix { get; set; }
        public string Group_suffix { get; set; }

        public List<object> Alias = new List<object>();
        public List<object> Employments = new List<object>();
        public List<object> _myEmployments = new List<object>();

        public bool isPrimaryPosition;

        public List<object> LoginId = new List<object>();
        public List<string> SecondManager;
        public List<string> SecondChartName;
        public List<string> SecondChartId;
        public List<string> SecondUnitName;
        public List<string> SecondUnitId;

    }
}
