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
using static FimSync_Ezma.Constants;

namespace FimSync_Ezma
{
    public class Employments : EmploymentAttributes
    {
        internal Microsoft.MetadirectoryServices.CSEntryChange GetCSEntryChange()
        {
            // Add all to CSEntry
            CSEntryChange csentry = CSEntryChange.Create();
            csentry.ObjectModificationType = ObjectModificationType.Add;
            csentry.ObjectType = SchemaTypes.employments;

            // EmploymentId, E_EmployeeId and E_HRMID must always be present
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.EmploymentIdDN, EmploymentId));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.E_EmployeeId, E_EmployeeId));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.E_HRMID, E_HRMID));

            if (DateFrom != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.DateFrom, DateFrom));
            if (DateTo != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.DateTo, DateTo));
            if (OrgUnitId != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.OrgUnitId, OrgUnitId));
            if (OrgUnitDescription != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.OrgUnitDescription, OrgUnitDescription));
            if (PostCode != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.PostCode, PostCode));
            if (PostCodeDescription != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.PostCodeDescription, PostCodeDescription));

            // bool has always a value
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.MainPosition, MainPosition));
            if (E_ManagerId != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.E_ManagerId, E_ManagerId));
            if (PositionTitle != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.PositionTitle, PositionTitle));
            if (PositionId != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.PositionId, PositionId));
            if (PositionType != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.PositionType, PositionType));
            if (PositionTypeValue != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.E_positionTypeValue, PositionTypeValue));
            if (PositionTypeName != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.E_positionTypeName, PositionTypeName));
            if (PositionCodeName != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.E_positionCodeName, PositionCodeName));
            if (PositionCodeValue != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.E_positionCodeValue, PositionCodeValue));
            if (PositionPercentage != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.PositionPercentage, PositionPercentage));
            if (PositionDescription != null)
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.PositionDescription, PositionDescription));

            return csentry;
        }
    }
}
