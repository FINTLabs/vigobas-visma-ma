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
    class Task
    {
        // Definisjoner for bruk i koden
        string taskid;
        Dictionary<string, List<string>> _tasks = new Dictionary<string, List<string>>();
        Dictionary<string, string> _tasknames = new Dictionary<string, string>();
        
        public Task(string taskkey, List<string> taskvalues, Dictionary<string, string> tasknames)
        {
            _tasks.Add(taskkey, taskvalues);
            _tasknames = tasknames;
            taskid = taskkey;
        }
        
        public string Anchor()
        {
            return taskid;
        }

        public override string ToString()
        {
            return taskid;
        }

        //internal Microsoft.MetadirectoryServices.CSEntryChange GetCSEntryChange()
        //{
        //    CSEntryChange csentry = CSEntryChange.Create();
        //    csentry.ObjectModificationType = ObjectModificationType.Add;
        //    csentry.ObjectType = "task";
            
        //    foreach (var task in _tasks)
        //    {
        //        csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("taskid", task.Key));
        //        csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("task description", _tasknames[task.Key].ToString()));
                
        //        IList<object> emp = new List<object>();
        //        foreach (var employee in task.Value)
        //        {
        //            emp.Add(employee.ToString());
        //        }
        //        csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("employee Id", emp));
        //    }
            
        //    return csentry;

        //}
    }
}
