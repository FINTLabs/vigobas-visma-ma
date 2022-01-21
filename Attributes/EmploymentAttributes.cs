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

namespace FimSync_Ezma
{
    public class EmploymentAttributes
    {
        public string EmploymentId { get; set; }
        public string E_EmployeeId { get; set; }
        public string E_HRMID { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string OrgUnitId { get; set; }
        public string OrgUnitDescription { get; set; }
        public string PostCode { get; set; }
        public string PostCodeDescription { get; set; }
        public bool MainPosition { get; set; }
        public string E_ManagerId { get; set; }
        public string PositionTitle { get; set; }
        public string PositionId { get; set; }
        public string PositionType { get; set; }
        public string PositionPercentage { get; set; }
        public string PositionDescription { get; set; }
        public string PositionTypeName { get; set; }
        public string PositionTypeValue { get; set; }
        public string PositionCodeName { get; set; }
        public string PositionCodeValue { get; set; }
    }
}
