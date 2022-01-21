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
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System;
using static FimSync_Ezma.Constants;

namespace FimSync_Ezma
{
    class Group : GroupAttributes
    {
        public Group(string groupKey, List<string> groupMembers, string groupName, string groupDescription, KeyedCollection<string, ConfigParameter> configparameter)
        {
            _groups.Add(groupKey, Tuple.Create(groupMembers, groupName, groupDescription));
            group_prefix = configparameter["Gruppeprefix"].Value;
            group_suffix = configparameter["Gruppesuffix"].Value;
        }

        // Returns CSEntryChange for use by the FIM Sync Engine directly
        internal Microsoft.MetadirectoryServices.CSEntryChange GetCSEntryChange()
        {
            CSEntryChange csentry = CSEntryChange.Create();
            csentry.ObjectModificationType = ObjectModificationType.Add;
            csentry.ObjectType = SchemaTypes.group;

            foreach (var _group in _groups)
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.groupIdDN, group_prefix + _group.Key + group_suffix));
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.groupName, _groups[_group.Key].Item2.ToString()));
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.groupDescription, _groups[_group.Key].Item3.ToString()));
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.groupSourceId, _group.Key));
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.groupType, _groups[_group.Key].Item3.ToString()));

                IList <object> members = new List<object>();
                foreach (var member in _groups[_group.Key].Item1)
                {
                    members.Add(member.ToString());
                }
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(SchemaAttributes.members, members));
            }

            return csentry;
        }
    }
}
