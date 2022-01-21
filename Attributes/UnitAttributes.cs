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
    public class UnitAttributes
    {
        public string UnitId { get; set; }
        public string UnitCompany { get; set; }
        public string UnitOrgCode { get; set; }
        public string UnitName { get; set; }
        public string UnitParent { get; set; }
        public string ManagerId { get; set; }
        public string ManagerName { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string PostalAddress1 { get; set; }
        public string PostalAddress2 { get; set; }
        public string PostalCode { get; set; }
        public string PostalArea { get; set; }
        public string VisitAddress1 { get; set; }
        public string VisitAddress2 { get; set; }
        public string VisitPostalCode { get; set; }
        public string VisitPostalArea { get; set; }
        public string Email { get; set; }
    }
}
