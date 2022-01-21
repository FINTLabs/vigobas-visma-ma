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
using System.Text;
//using System.Threading.Tasks;
using System.Net.Http;
using System.Web;
using System.IO;
using Microsoft.MetadirectoryServices;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Net;
using Vigo.Bas.ManagementAgent.Log;
using System.Diagnostics;
using static FimSync_Ezma.Constants;
using System.Linq;

namespace FimSync_Ezma
{
    public class EzmaExtension :
    IMAExtensible2CallExport,
    IMAExtensible2CallImport,
    IMAExtensible2GetSchema,
    IMAExtensible2GetCapabilities,
    IMAExtensible2GetParameters
    {
        #region Page Size
        // Variables for page size
        private int m_importDefaultPageSize = 12;
        private int m_importMaxPageSize = 50;
        private int m_exportDefaultPageSize = 10;
        private int m_exportMaxPageSize = 20;
        
        public int ImportMaxPageSize
        {
            get
            {
                return m_importMaxPageSize;
            }
        }
        public int ImportDefaultPageSize
        {
            get
            {
                return m_importDefaultPageSize;
            }
        }
        public int ExportDefaultPageSize
        {
            get
            {
                return m_exportDefaultPageSize;
            }
            set
            {
                m_exportDefaultPageSize = value;
            }
        }
        public int ExportMaxPageSize
        {
            get
            {
                return m_exportMaxPageSize;
            }
            set
            {
                m_exportMaxPageSize = value;
            }
        }

        #endregion

        #region Div needed during import/export

        private string _from_employeeID;
        private string _to_employeeID;

        // Delclared to be used in export
        XDocument xmlExport;
        PersonAttributes ExportAttributes;

        // Enable if needed
        //List<string> AllEmployments = new List<string>();

        // List of persons allready imported. To prevent duplicates
        List<string> allreadyimported = new List<string>();

        // Create a Dictionary with Tuple Contianing List of members, GroupName, GroupDescription
        public Dictionary<string, Tuple<List<string>, string, string>> groups = new Dictionary<string, Tuple<List<string>, string, string>>();

        // Map unitID to UnitNumber
        public Dictionary<string, string> unitIdToUnitNumber = new Dictionary<string, string>();

        // Map personIDHRM to userId
        public Dictionary<string, string> persIDHRM_userId = new Dictionary<string, string>();

        // List of Companies
        private List<string> companies = new List<string>();

        // List ignored categories
        public List<string> ignored_categories = new List<string>();

        // List ignored positionTypes
        public List<string> ignored_positionTypes = new List<string>();

        #endregion  

        #region Capabilities, Config parameters and Schema
        public EzmaExtension()
        {

        }

        public MACapabilities Capabilities
        {
            // Returns the capabilities that this management agent has
            get
            {
                MACapabilities myCapabilities = new MACapabilities();

                myCapabilities.ConcurrentOperation = true;
                myCapabilities.ObjectRename = false;
                myCapabilities.DeleteAddAsReplace = false;
                myCapabilities.DeltaImport = false;
                myCapabilities.DistinguishedNameStyle = MADistinguishedNameStyle.None;
                myCapabilities.ExportType = MAExportType.ObjectReplace;
                myCapabilities.NoReferenceValuesInFirstExport = true;
                myCapabilities.Normalizations = MANormalizations.None;
                myCapabilities.ObjectConfirmation = MAObjectConfirmation.NoDeleteConfirmation;

                return myCapabilities;
            }
        }

        public IList<ConfigParameterDefinition> GetConfigParameters(KeyedCollection<string, ConfigParameter> configParameters, ConfigParameterPage page)
        {
            List<ConfigParameterDefinition> configParametersDefinitions = new List<ConfigParameterDefinition>();

            switch (page)
            {
                case ConfigParameterPage.Connectivity:
                    // Parameters
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter("Hostname", "", "vismaweb01.hedmark.org"));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter("Port", "", "8090"));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateDropDownParameter("Protocol", "http,https", false, "http"));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter("Timeout (sek)", "", "600"));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateDropDownParameter("Export type", "Simple,Advanced", false, "Simple"));
                    //configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter("Webservice url", "", "http://vismaweb01.hedmark.org:8090/hrm_ws/"));
                    //configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter("Export simple persons url", "", "http://vismaweb01.hedmark.org:8090/enterprise_ws/secure/user/"));

                    // Username and Password
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter("Username", ""));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateEncryptedStringParameter("Password", ""));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter("Domain", ""));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateDividerParameter());
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter("Selskap", "", "1"));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateLabelParameter("Selskaper delt med komma ,"));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateCheckBoxParameter("Prefix personDN med selskapsnummer", false));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter("Fra ansattnummer", "", "10000"));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter("Til ansattnummer", "", "25000"));
                    
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateCheckBoxParameter("Logg med debug?", false));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateCheckBoxParameter("Hent personer frem i tid", false));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter("Antall dager frem i tid", "", "60"));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateDividerParameter());
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter("Ignorer ansattkategorier", ""));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateDividerParameter());
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter("Ignorer stillingstype", ""));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateDividerParameter());
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateCheckBoxParameter("Hent organisasjoninfo", false));
                    //configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter("Dato Offset", "", "0"));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateDividerParameter());

                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateCheckBoxParameter("Lag grupper", false));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateLabelParameter("Pre eller suffix gruppetyper"));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter("Gruppeprefix", "", ""));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter("Gruppesuffix", "", ""));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateLabelParameter("Velg gruppetyper"));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateCheckBoxParameter("Unit grupper", true));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateCheckBoxParameter("Dimension grupper", true));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateCheckBoxParameter("BusinessNumber grupper", true));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateCheckBoxParameter("Company grupper", true));
                    //configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter("Export advanced persons url", "", "http://vismaweb01.hedmark.org:8090/hrm_ws/secure/persons/import/"));



                    break;

                case ConfigParameterPage.Global:
                case ConfigParameterPage.Partition:
                case ConfigParameterPage.RunStep:
                    break;
            }

            return configParametersDefinitions;
        }

        public ParameterValidationResult ValidateConfigParameters(KeyedCollection<string, ConfigParameter> configParameters, ConfigParameterPage page)
        {

            // Configuration validation

            ParameterValidationResult myResults = new ParameterValidationResult();

            // Try connect to VismaWebService. Stops if cannot connect
            try
            {
                //RestartLog(configParameters);
                String AllPersons = ValidateVismaWebservice(configParameters);
                XDocument.Parse(AllPersons);
            }
            catch (Exception e)
            {
                myResults.ErrorMessage = "Error message: " + e.Message;
                myResults.Code = ParameterValidationResultCode.Failure;
            }

            return myResults;
        }

        public Schema GetSchema(KeyedCollection<string, ConfigParameter> configParameters)
        {
            // Create CS Schema type person
            SchemaType person = SchemaType.Create(SchemaTypes.person, false);

            // Anchor
            person.Attributes.Add(SchemaAttribute.CreateAnchorAttribute(SchemaAttributes.personDN, AttributeType.String));


            // Attributes
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.personIdHRM, AttributeType.String, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.givenName, AttributeType.String, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.familyName, AttributeType.String, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.ssn, AttributeType.String, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.dateOfBirth, AttributeType.String, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.genderCode, AttributeType.String, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(SchemaAttributes.alias, AttributeType.String, AttributeOperation.ImportExport));
            person.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(SchemaAttributes.loginID, AttributeType.String, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(SchemaAttributes.initials, AttributeType.String, AttributeOperation.ImportExport));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.employeeId, AttributeType.String, AttributeOperation.ImportExport));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.userId, AttributeType.String, AttributeOperation.ImportExport));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.manager, AttributeType.Reference, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.personManageridAsString, AttributeType.String, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.workTasks, AttributeType.String, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.jobTitle, AttributeType.String, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.startDate, AttributeType.String, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.endDate, AttributeType.String, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.email, AttributeType.String, AttributeOperation.ImportExport));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.privateEmail, AttributeType.String, AttributeOperation.ImportExport));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.workMobile, AttributeType.String, AttributeOperation.ImportExport));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.workPhone, AttributeType.String, AttributeOperation.ImportExport));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.privatePhone, AttributeType.String, AttributeOperation.ImportExport));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.privateMobile, AttributeType.String, AttributeOperation.ImportExport));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.category_id, AttributeType.String, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.categorydescription, AttributeType.String, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.lastChangedDate, AttributeType.String, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.employmentPercentage, AttributeType.String, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.businessNumbername, AttributeType.String, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.businessNumbervalue, AttributeType.String, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.companyId, AttributeType.String, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.companyName, AttributeType.String, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.organizationNumber, AttributeType.String, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.companyNumbername, AttributeType.String, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.companyNumbervalue, AttributeType.String, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.positionPercentage, AttributeType.String, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.positionStartDate, AttributeType.String, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.positionEndDate, AttributeType.String, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.locationName, AttributeType.String, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.locationId, AttributeType.String, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.yearlyHours, AttributeType.String, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.positionCode, AttributeType.String, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.positionId, AttributeType.String, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.tableNr, AttributeType.String, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.positionCodeName, AttributeType.String, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.positionTypeValue, AttributeType.String, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.positionTypeName, AttributeType.String, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.publicPositionCodeName, AttributeType.String, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.publicPositionCodeValue, AttributeType.String, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.dimension2name, AttributeType.String, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.dimension2value, AttributeType.String, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.dimension3name, AttributeType.String, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.dimension3value, AttributeType.String, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(SchemaAttributes.employments, AttributeType.Reference, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(SchemaAttributes.employmentsAsString, AttributeType.String, AttributeOperation.ImportOnly));


            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.chartId, AttributeType.String, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.chartName, AttributeType.String, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.unitId, AttributeType.String, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.unitIdRef, AttributeType.Reference, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.unitName, AttributeType.String, AttributeOperation.ImportOnly));

            person.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(SchemaAttributes.secondmanager, AttributeType.Reference, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(SchemaAttributes.secondchartName, AttributeType.String, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(SchemaAttributes.secondchartId, AttributeType.String, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(SchemaAttributes.secondunitName, AttributeType.String, AttributeOperation.ImportOnly));
            person.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(SchemaAttributes.secondunitId, AttributeType.String, AttributeOperation.ImportOnly));


            // Create CS type Group
            SchemaType gruppe = SchemaType.Create(SchemaTypes.group, false);

            // Define Anchor
            gruppe.Attributes.Add(SchemaAttribute.CreateAnchorAttribute(SchemaAttributes.groupIdDN, AttributeType.String, AttributeOperation.ImportOnly));

            // Attributes
            gruppe.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.groupName, AttributeType.String, AttributeOperation.ImportOnly));
            gruppe.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.groupDescription, AttributeType.String, AttributeOperation.ImportOnly));
            gruppe.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.groupSourceId, AttributeType.String, AttributeOperation.ImportOnly));
            gruppe.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(SchemaAttributes.members, AttributeType.Reference, AttributeOperation.ImportOnly));
            gruppe.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.groupType, AttributeType.String, AttributeOperation.ImportOnly));


            // Create unit
            SchemaType unit = SchemaType.Create(SchemaTypes.unit, true);

            // Define Anchor
            unit.Attributes.Add(SchemaAttribute.CreateAnchorAttribute(SchemaAttributes.unitorgkodeDN, AttributeType.String, AttributeOperation.ImportOnly));

            // Attributes
            unit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.unitname, AttributeType.String, AttributeOperation.ImportOnly));
            unit.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(SchemaAttributes.employee_id, AttributeType.Reference, AttributeOperation.ImportOnly));
            unit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.unitcompany, AttributeType.String, AttributeOperation.ImportOnly));
            unit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.unitid, AttributeType.String, AttributeOperation.ImportOnly));
            unit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.unitparent, AttributeType.Reference, AttributeOperation.ImportOnly));
            unit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.unitparentAsString, AttributeType.String, AttributeOperation.ImportOnly));
            unit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.managerid, AttributeType.Reference, AttributeOperation.ImportOnly));
            unit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.manageridAsString, AttributeType.String, AttributeOperation.ImportOnly));
            unit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.managername, AttributeType.String, AttributeOperation.ImportOnly));
            unit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.phone, AttributeType.String, AttributeOperation.ImportOnly));
            unit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.fax, AttributeType.String, AttributeOperation.ImportOnly));
            unit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.postaladdress1, AttributeType.String, AttributeOperation.ImportOnly));
            unit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.postaladdress2, AttributeType.String, AttributeOperation.ImportOnly));
            unit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.postalcode, AttributeType.String, AttributeOperation.ImportOnly));
            unit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.postalarea, AttributeType.String, AttributeOperation.ImportOnly));
            unit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.visitaddress1, AttributeType.String, AttributeOperation.ImportOnly));
            unit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.visitaddress2, AttributeType.String, AttributeOperation.ImportOnly));
            unit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.visitpostalcode, AttributeType.String, AttributeOperation.ImportOnly));
            unit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.visitpostalarea, AttributeType.String, AttributeOperation.ImportOnly));
            unit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.unitemail, AttributeType.String, AttributeOperation.ImportOnly));

            //// Create employments
            SchemaType employments = SchemaType.Create(SchemaTypes.employments, true);

            //// Set Anchor employments
            employments.Attributes.Add(SchemaAttribute.CreateAnchorAttribute(SchemaAttributes.EmploymentIdDN, AttributeType.String, AttributeOperation.ImportOnly));

            //Attributes
            employments.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.E_EmployeeId, AttributeType.String, AttributeOperation.ImportOnly));
            employments.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.E_HRMID, AttributeType.String, AttributeOperation.ImportOnly));
            employments.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.DateFrom, AttributeType.String, AttributeOperation.ImportOnly));
            employments.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.DateTo, AttributeType.String, AttributeOperation.ImportOnly));
            employments.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.OrgUnitId, AttributeType.String, AttributeOperation.ImportOnly));
            employments.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.OrgUnitDescription, AttributeType.String, AttributeOperation.ImportOnly));
            employments.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.PostCode, AttributeType.String, AttributeOperation.ImportOnly));
            employments.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.PostCodeDescription, AttributeType.String, AttributeOperation.ImportOnly));
            employments.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.MainPosition, AttributeType.Boolean, AttributeOperation.ImportOnly));
            employments.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.E_ManagerId, AttributeType.Reference, AttributeOperation.ImportOnly));
            employments.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.PositionTitle, AttributeType.String, AttributeOperation.ImportOnly));
            employments.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.PositionId, AttributeType.String, AttributeOperation.ImportOnly));
            employments.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.PositionType, AttributeType.String, AttributeOperation.ImportOnly));
            employments.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.E_positionTypeName, AttributeType.String, AttributeOperation.ImportOnly));
            employments.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.E_positionTypeValue, AttributeType.String, AttributeOperation.ImportOnly));
            employments.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.E_positionCodeName, AttributeType.String, AttributeOperation.ImportOnly));
            employments.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.E_positionCodeValue, AttributeType.String, AttributeOperation.ImportOnly));
            employments.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.PositionPercentage, AttributeType.String, AttributeOperation.ImportOnly));
            employments.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaAttributes.PositionDescription, AttributeType.String, AttributeOperation.ImportOnly));


            // Return schema
            Schema schema = Schema.Create();
            schema.Types.Add(person);
            schema.Types.Add(gruppe);
            schema.Types.Add(unit);
            schema.Types.Add(employments);

            return schema;
        }
        #endregion

        #region Private REST and WS methods
        private NetworkCredential VismaGetNetworkCredential(KeyedCollection<string, ConfigParameter> configParameters)
        {
            //Collect username and password, domain and return to connect to WS
            string username = null;
            if (configParameters["Domain"].Value != null && configParameters["Domain"].Value != "")
            {
                username = configParameters["Domain"].Value + "\\" + configParameters["Username"].Value;
            }
            else
            {
                username = configParameters["Username"].Value;
            }
            return new NetworkCredential(username, configParameters["Password"].SecureValue);
        }


        private String VismaGetAllActivePersons(KeyedCollection<string, ConfigParameter> configParameters, string selskap)
        {
            //Define from - to employeeNumber
            _from_employeeID = configParameters["Fra ansattnummer"].Value;
            _to_employeeID = configParameters["Til ansattnummer"].Value;

            string _serverportprotocol = configParameters["Protocol"].Value + "://" + configParameters["Hostname"].Value + ":" + configParameters["Port"].Value;

            Logger.Log.Info("Henter personer fra ansattnr " + _from_employeeID + " til " + _to_employeeID);

            Logger.Log.Info(_serverportprotocol + "/hrm_ws/secure/persons/company/" + selskap
                        + "/start-id/" + _from_employeeID + "/end-id/" + _to_employeeID);

            // Send generic request for all users with credentials, and return the string containing the XML result
            return RESTGetGenericRequest(
                _serverportprotocol + "/hrm_ws/secure/persons/company/" + selskap
                + "/start-id/" + _from_employeeID + "/end-id/" + _to_employeeID,
                VismaGetNetworkCredential(configParameters),
                "GET",
                null,
                int.Parse(configParameters["Timeout (sek)"].Value));

        }

        private String ValidateVismaWebservice(KeyedCollection<string, ConfigParameter> configParameters)
        {
            //Define from - to employeeNumber
            _from_employeeID = configParameters["Fra ansattnummer"].Value;
            _to_employeeID = configParameters["Til ansattnummer"].Value;
            string _serverportprotocol = configParameters["Protocol"].Value + "://" + configParameters["Hostname"].Value + ":" + configParameters["Port"].Value;


            //Convert string to int and add + 10
            if (Convert.ToInt32(_from_employeeID) < Convert.ToInt32(_to_employeeID) && Convert.ToInt32(_to_employeeID) - Convert.ToInt32(_from_employeeID) >= 10)
            {
                _to_employeeID = (Convert.ToInt32(_from_employeeID) + 10).ToString();
            }

            //Is there more than one Company
            if (configParameters["Selskap"].Value.Contains(","))
            {
                foreach (string selskap in configParameters["Selskap"].Value.Split(','))
                {
                    companies.Add(selskap);
                }
            }
            else
            {
                companies.Add(configParameters["Selskap"].Value);
            }

            //If more than one employee is defined, validate against one the first ten employeeNumbers to make Validation faster


            // Send generic request for all users with credentials, and return the string containing the XML result
            return RESTGetGenericRequest(
                    _serverportprotocol + "/hrm_ws/secure/persons/company/" + companies[0]
                + "/start-id/" + _from_employeeID + "/end-id/" + _to_employeeID,
                VismaGetNetworkCredential(configParameters),
                "GET",
                null,
                int.Parse(configParameters["Timeout (sek)"].Value)
            );

        }

        private String VismaGetAllFuturePersons(KeyedCollection<string, ConfigParameter> configParameters, string selskap)
        {
            //Define days ahead for future persons
            string tomorrow = DateTime.Now.AddDays(+1).ToString("yyyy-MM-dd");
            int futuredays = Convert.ToInt32(configParameters["Antall dager frem i tid"].Value);
            string futuredate = DateTime.Now.AddDays(+futuredays).ToString("yyyy-MM-dd");
            string _serverportprotocol = configParameters["Protocol"].Value + "://" + configParameters["Hostname"].Value + ":" + configParameters["Port"].Value;

            Logger.Log.Info("Henter personer " + futuredays + " dager frem i tid, fra " + tomorrow + " til " + futuredate);

            // Send generic request for all users with credentials, and return the string containing the XML result
            return RESTGetGenericRequest(
                _serverportprotocol + "/hrm_ws/secure/persons/company/" + selskap
            + "/not-started/date-interval/" + tomorrow + "/" + futuredate,
                VismaGetNetworkCredential(configParameters),
                "GET",
                null,
                int.Parse(configParameters["Timeout (sek)"].Value)
            );
        }

        private String VismaGetAllOrg(KeyedCollection<string, ConfigParameter> configParameters, string company)
        {

            //Dato Offset
            //int dateOffset = int.Parse(configParameters["Dato Offset"].Value) * -1;
            string chartDate = DateTime.Now.ToString(dateFormat);
            string _serverportprotocol = configParameters["Protocol"].Value + "://" + configParameters["Hostname"].Value + ":" + configParameters["Port"].Value;

            string orgChartURL = _serverportprotocol + "/hrm_ws/secure/organization/chart/" + chartDate + "/" + company + "/";

            Logger.Log.Info("Henter organisasjonsinfo fra url " + orgChartURL);

            // Send generic request for all users with credentials, and return the string containing the XML result
            return RESTGetGenericRequest(
                orgChartURL,
                VismaGetNetworkCredential(configParameters),
                "GET",
                null,
                int.Parse(configParameters["Timeout (sek)"].Value)
            );
        }

        private string VismaGetUserID (KeyedCollection<string, ConfigParameter> configParameters, string personDN)
        {
            //Dato Offset
            string _serverportprotocol = configParameters["Protocol"].Value + "://" + configParameters["Hostname"].Value + ":" + configParameters["Port"].Value;

            string URL = _serverportprotocol + "/hrm_ws/secure/persons/id/" + personDN;

            Logger.Log.Info("Henter userId fra url " + URL);

            // Send generic request for all users with credentials, and return the string containing the XML result
            return RESTGetGenericRequest(
                URL,
                VismaGetNetworkCredential(configParameters),
                "GET",
                null,
                int.Parse(configParameters["Timeout (sek)"].Value)
            );

        }

        private String RESTGetGenericRequest(string uri, NetworkCredential credentials, string method, string body, int timeoutSec)
        {
            try
            {
                // Log request
                //Logger.Log.Info("Request: " + uri);
                //Logger.Log.Info("Request method: " + method);

                // Create request and assign credentials
                WebRequest request = WebRequest.Create(uri);
                request.Method = method;
                request.Timeout = timeoutSec * 1000;

                // Send POST body
                if (method.ToUpper() == Method.POST && body != null)
                {
                    //Logger.Log.Info("Parsing body: " + body);
                    byte[] bytes = System.Text.Encoding.ASCII.GetBytes(body);
                    request.ContentLength = bytes.Length;
                    request.ContentType = "application/x-www-form-urlencoded";

                    Stream os = request.GetRequestStream();
                    os.Write(bytes, 0, bytes.Length); //Push it out there
                    os.Close();
                }

                if (credentials != null)
                {
                    //Logger.Log.Info("Username: " + credentials.UserName);
                    //Logger.Log.Info("Domain: " + credentials.Domain);

                    request.Credentials = credentials;
                }
                // Even If certificates not is valid, it moves on
                System.Net.Security.RemoteCertificateValidationCallback BadCertificates = new System.Net.Security.RemoteCertificateValidationCallback(delegate { return true; });
                System.Net.ServicePointManager.ServerCertificateValidationCallback = BadCertificates;
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

                // Get WebResponse, create Stream and read the stream with a StreamReader
                WebResponse response = request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);

                // Read all content from stream
                string responseFromServer = reader.ReadToEnd();

                // Cleanup the reader, stream and response.
                reader.Close();
                responseStream.Close();
                response.Close();

                // Return, but remove the namespace
                return responseFromServer;
            }
            catch (Exception e)
            {
                //Logger.Log.Error("Message: " + e.Message);
                //Logger.Log.Error("StackTrace: " + e.StackTrace);
                throw new Exception("URL: " + uri + " Message: " + e.Message + " StackTrace: " + e.StackTrace);
            }
        }
        #endregion
        
        #region Import methods
        /*
         * Attributes used during import 
         */
        List<ImportListItem> ImportedObjectsList;
        int GetImportEntriesIndex, GetImportEntriesPageSize;

        public OpenImportConnectionResults OpenImportConnection(KeyedCollection<string, ConfigParameter> configParameters,
                                                Schema types,
                                                OpenImportConnectionRunStep importRunStep)


        {
            //Add Companies to a list
            if (configParameters["Selskap"].Value.Contains(","))
            {
                foreach (string company in configParameters["Selskap"].Value.Split(','))
                {
                    companies.Add(company);
                }
            }
            else
            {
                companies.Add(configParameters["Selskap"].Value);
            }




            if (configParameters["Ignorer ansattkategorier"].Value.Contains(","))
            {
                foreach (string position in configParameters["Ignorer ansattkategorier"].Value.Split(','))
                {
                    ignored_categories.Add(position);
                }
            }
            else
            {
                ignored_categories.Add(configParameters["Ignorer ansattkategorier"].Value);
            }

            
            if (configParameters["Ignorer stillingstype"].Value.Contains(","))
            {
                foreach(string position in configParameters["Ignorer stillingstype"].Value.Split(','))
                {
                    ignored_positionTypes.Add(position);
                }

            }
            else
            {
                ignored_positionTypes.Add(configParameters["Ignorer stillingstype"].Value);
            }
            
            
            
            
            Logger.Log.Info("Starter import");
            // Instantiate ImportedObjectsList
            ImportedObjectsList = new List<ImportListItem>();

            //Getting Users and Groups foreach Company
            foreach (string company in companies)
            {

                String _VismaGetAllPersons = VismaGetAllActivePersons(configParameters, company);
                foreach (XElement xperson in XDocument.Parse(_VismaGetAllPersons).Element("personsXML").Elements("person"))
                {
                    try
                    {
                        string userid = xperson.Element("authentication").Element(SchemaAttributes.userId)?.Value;
                        string personIDHRM = company + "-" + xperson.Attribute(SchemaAttributes.personIdHRM)?.Value;
                        if (userid.Length > 0 && personIDHRM.Length > 0)
                        {
                            if (!persIDHRM_userId.ContainsKey(personIDHRM))
                            {
                                persIDHRM_userId.Add(personIDHRM, userid);
                            }
                        }

                    }
                    catch
                    { }
                }


                string id = "";
                // Get organization information from own WS 
                if (configParameters["Hent organisasjoninfo"].Value == "1")
                {
                    //Get Organizations
                    Stopwatch getOrganizationsStopwatch = new Stopwatch();
                    getOrganizationsStopwatch.Start();

                    try
                    {
                        String _VismaGetAllOrg = VismaGetAllOrg(configParameters, company);

                        getOrganizationsStopwatch.Stop();
                        Logger.Log.Info(string.Format("Webservice kallet GetOrganizations fullførte på {0} minutter og {1} sekunder", getOrganizationsStopwatch.Elapsed.Minutes, getOrganizationsStopwatch.Elapsed.Seconds));

                        foreach (XElement xunit in XDocument.Parse(_VismaGetAllOrg).Element("charts").Element("chart").Element("units").Elements("unit"))
                        {
                            try
                            {
                                if (!allreadyimported.Contains("unit" + company + "-" + xunit.Attribute("id").Value))
                                {
                                    id = xunit.Attribute("id").Value;
                                    // Parse as Organization
                                    Unit unit = new Unit(xunit, unitIdToUnitNumber, configParameters, company, persIDHRM_userId);
                                    if (configParameters["Logg med debug?"].Value == "1")
                                    {
                                        Logger.Log.Debug("La til organisasjon: " + xunit.Attribute("id").Value);
                                    }
                                    ImportedObjectsList.Add(new ImportListItem() { unit = unit });
                                    allreadyimported.Add("unit" + company + "-" + xunit.Attribute("id").Value);
                                }
                            }
                            catch
                            {
                                Logger.Log.ErrorFormat("Feilet import på unit med id: {0}", xunit.Attribute("id").Value);
                            }
                        }
                    }
                    catch
                    {
                        Logger.Log.Error("Kunne ikke hente ut units fra webservice. Sjekk om Uri returnerer noen units");
                    }
                }

                Stopwatch getAllPersonsStopwatch = new Stopwatch();
                getAllPersonsStopwatch.Start();
                // Get users from WS

                try
                {
                    //String _VismaGetAllPersons = VismaGetAllActivePersons(configParameters, company);

                    getAllPersonsStopwatch.Stop();
                    Logger.Log.Info(string.Format("Webservice kallet GetAllPersons fullførte på {0} minutter og {1} sekunder", getAllPersonsStopwatch.Elapsed.Minutes, getAllPersonsStopwatch.Elapsed.Seconds));

                    // Parse all to get dictionary of personIDHRM to userId to use in Manager-ref

                    foreach (XElement xperson in XDocument.Parse(_VismaGetAllPersons).Element("personsXML").Elements("person"))
                    {
                        try
                        {
                            string userid = xperson.Element("authentication").Element(SchemaAttributes.userId)?.Value;
                            string personIDHRM = company + "-" + xperson.Attribute(SchemaAttributes.personIdHRM)?.Value;
                            if (userid.Length > 0 && personIDHRM.Length > 0)
                            {
                                if (!persIDHRM_userId.ContainsKey(personIDHRM))
                                {
                                    persIDHRM_userId.Add(personIDHRM, userid);
                                }
                            }

                        }
                        catch
                        { }
                    }

                    // Parse all elements in /personsXML/person
                    foreach (XElement xperson in XDocument.Parse(_VismaGetAllPersons).Element("personsXML").Elements("person"))
                    {
                        try
                        {
                            if (!allreadyimported.Contains(company + "-" + xperson.Attribute(SchemaAttributes.personIdHRM).Value))
                            {
                                if (!ignored_categories.Contains(xperson.Element("employments")?.Element("employment")?.Element("category")?.Attribute("id")?.Value ?? ""))
                                {
                                    //id = xperson.Element("employments").Element("employment").Element("category").Attribute("id").Value;
                                    // Parse as Person
                                    //Person person = new Person(xperson, groups, configParameters, company, ImportedObjectsList, unitIdToUnitNumber, AllEmployments);
                                    Person person = new Person(xperson, groups, configParameters, company, ImportedObjectsList, unitIdToUnitNumber, allreadyimported, persIDHRM_userId, ignored_positionTypes);
                                    ImportedObjectsList.Add(new ImportListItem() { person = person });
                                    if (configParameters["Logg med debug?"].Value == "1")
                                    {
                                        Logger.Log.Debug(string.Format("La til person: ({0}) {1} {2}, i selskap {3}", xperson.Attribute(SchemaAttributes.personIdHRM).Value, xperson.Element(SchemaAttributes.givenName).Value, xperson.Element(SchemaAttributes.familyName).Value, company));
                                    }
                                    allreadyimported.Add(company + "-" + xperson.Attribute(SchemaAttributes.personIdHRM).Value);
                                                                             
                                }
                            }
                        }
                        catch
                        {
                            Logger.Log.ErrorFormat("Feil på  på person med personIdHRM: {0}. Personen ble ikke importert", xperson.Attribute(SchemaAttributes.personIdHRM).Value);
                        }
                    }
                }
                catch
                {
                    Logger.Log.Error("Kunne ikke hente ut personer fra webservice. Sjekk at Uri inneholder personer");
                }

                if (configParameters["Hent personer frem i tid"].Value == "1")
                {
                    Logger.Log.Info("Henter personer frem i tid");
                    // Get future persons

                    try
                    {
                        Stopwatch getFuturePersonsStopwatch = new Stopwatch();
                        getFuturePersonsStopwatch.Start();


                        String _VismaGetAllFuturePersons = VismaGetAllFuturePersons(configParameters, company);

                        getFuturePersonsStopwatch.Stop();
                        Logger.Log.Info(string.Format("Webservice kallet GetFuturePersons fullførte på {0} minutter og {1} sekunder", getFuturePersonsStopwatch.Elapsed.Minutes, getFuturePersonsStopwatch.Elapsed.Seconds));

                        // Parse all elements in /personsXML/person as future person
                        foreach (XElement xperson in XDocument.Parse(_VismaGetAllFuturePersons).Element("personsXML").Elements("person"))
                        {
                            try
                            {
                                // Check if person allready imported
                                if (!allreadyimported.Contains(company + "-" + xperson.Attribute(SchemaAttributes.personIdHRM).Value))
                                {
                                    //Check if person has "Ingnorer annsattkategori" 
                                    if (!ignored_categories.Contains(xperson.Element("employments")?.Element("employment")?.Element("category")?.Attribute("id")?.Value ?? ""))
                                    { 
                                        // Parse as Person
                                        // Person person = new Person(xperson, groups, configParameters, company, ImportedObjectsList, unitIdToUnitNumber, AllEmployments);
                                        Person person = new Person(xperson, groups, configParameters, company, ImportedObjectsList, unitIdToUnitNumber, allreadyimported, persIDHRM_userId, ignored_positionTypes);
                                        ImportedObjectsList.Add(new ImportListItem() { person = person });
                                        if (configParameters["Logg med debug?"].Value == "1")
                                        {
                                            Logger.Log.Debug(string.Format("La til ikke startet person: ({0}) {1} {2}", xperson.Attribute(SchemaAttributes.personIdHRM).Value, xperson.Element("givenName").Value, xperson.Element("familyName").Value));
                                        }
                                        allreadyimported.Add(company + "-" + xperson.Attribute(SchemaAttributes.personIdHRM).Value);

                                    }
                                        
                                }
                            }
                            catch
                            {
                                Logger.Log.ErrorFormat("Feil på xml for person med personIdHRM: {0}. Personen ble ikke importert", xperson.Attribute(SchemaAttributes.personIdHRM).Value);
                            }

                            //can be implemented if needed
                            //else
                            //{
                            //    foreach (XElement position in xperson.Element("employments").Element("employment").Element("positions").Elements("position"))
                            //    {
                            //        string pos = null, unid = null;

                            //        pos = position.Element("positionInfo")?.Element("positionCode")?.Attribute("positionId")?.Value;
                            //        unid = position.Element("chart")?.Element("unit")?.Attribute("id")?.Value;

                            //        if (!string.IsNullOrEmpty(unid) && !string.IsNullOrEmpty(pos))
                            //        {
                            //            string _emp;
                            //            if (configParameters["Prefix personDN med selskapsnummer"].Value == "1")
                            //            {
                            //                _emp = company + "-" + xperson.Attribute("personIdHRM").Value + "-" + unid + "-" + pos;
                            //            }
                            //            else
                            //            {
                            //                _emp = xperson.Attribute("personIdHRM").Value + "-" + unid + "-" + pos;
                            //            }

                            //            if (!AllEmployments.Contains(_emp))
                            //            {
                            //                Employments emp = new Employments();

                            //                // Add Orgunitid and description to employments
                            //                emp.OrgUnitId = position.Element("chart").Element("unit")?.Attribute("id")?.Value;
                            //                emp.OrgUnitDescription = position.Element("chart").Element("unit")?.Attribute("name")?.Value;
                            //                emp.MainPosition = Convert.ToBoolean(position.Attribute("isPrimaryPosition")?.Value);
                            //                emp.E_EmployeeId = xperson.Element("employments").Element("employment").Element("employeeId")?.Value;
                            //                emp.EmploymentId = _emp;
                            //                AllEmployments.Add(emp.EmploymentId);
                            //                emp.E_ManagerId = emp.E_ManagerId = position.Element("chart").Element("unit").Element("manager")?.Attribute("id")?.Value;
                            //                emp.PositionPercentage = position.Element("positionPercentage")?.Value;
                            //                emp.DateFrom = position.Element("positionStartDate")?.Value;
                            //                emp.DateTo = position.Element("positionEndDate")?.Value;

                            //                XElement _positionInfo = position.Element("positionInfo");
                            //                XElement _positionCode = _positionInfo.Element("positionCode");
                            //                XElement _positionType = _positionInfo.Element("positionType");
                            //                XElement _publicPositionCode = _positionInfo.Element("publicPositionCode");
                            //                XElement _positionStatistics = position.Element("positionStatistics");
                            //                emp.PositionId = _positionCode?.Attribute("positionId")?.Value;
                            //                emp.PositionDescription = _publicPositionCode?.Attribute("name")?.Value;
                            //                emp.PositionType = _positionType?.Attribute("value")?.Value;

                            //                ImportedObjectsList.Add(new ImportListItem() { employments = emp });
                            //                if (configParameters["Logg med debug?"].Value == "1")
                            //                {
                            //                    Logger.Log.Debug(string.Format("La til employment {0} for ressursnummer {1}", emp.EmploymentId, emp.E_EmployeeId));
                            //                }
                            //            }
                            //        }
                            //    }

                            //}
                        }
                    }
                    catch
                    {
                        Logger.Log.Error("Kunne ikke hente ut fremtidige personer fra webservice. Sjekk at Uri inneholder personer");
                    }
                }

            }

            // Create groups from Unit values from person
            if (configParameters["Lag grupper"].Value == "1")
            {
                try
                {
                    Logger.Log.Info("Lager grupper");
                    foreach (var _group in groups)
                    {
                        if (!allreadyimported.Contains(configParameters["Gruppeprefix"].Value + _group.Key + configParameters["Gruppesuffix"].Value))
                        {
                            // Parse groups; key, members, Name, Description
                            Group group = new Group(_group.Key, _group.Value.Item1, _group.Value.Item2, _group.Value.Item3, configParameters);
                            if (configParameters["Logg med debug?"].Value == "1")
                            {
                                Logger.Log.Debug("La til gruppe: " + configParameters["Gruppeprefix"].Value + _group.Key + configParameters["Gruppesuffix"].Value);
                            }
                            ImportedObjectsList.Add(new ImportListItem() { group = group });
                            allreadyimported.Add(configParameters["Gruppeprefix"].Value + _group.Key + configParameters["Gruppesuffix"].Value);
                        }
                    }
                }
                catch
                {
                    Logger.Log.Error("Feil med generering av grupper. Sjekk om organisasjons webservice Uri inneholder organisasjoner");
                }
            }

            // Set index values and page size
            GetImportEntriesIndex = 0;
            GetImportEntriesPageSize = importRunStep.PageSize;

            return new OpenImportConnectionResults();
        }

        public CloseImportConnectionResults CloseImportConnection(CloseImportConnectionRunStep importRunStepInfo)
        {
            Logger.Log.Info("Import ferdig");
            return new CloseImportConnectionResults();
        }

        public GetImportEntriesResults GetImportEntries(GetImportEntriesRunStep importRunStep)
        {
            /* This method will be invoked multiple times, once for each "page" */

            List<CSEntryChange> csentries = new List<CSEntryChange>();
            GetImportEntriesResults importReturnInfo = new GetImportEntriesResults();

            // If no result, return empty success
            if (ImportedObjectsList == null || ImportedObjectsList.Count == 0)
            {
                importReturnInfo.CSEntries = csentries;
                return importReturnInfo;
            }

            // Parse a full page or to the end
            for (int currentPage = 0;
                GetImportEntriesIndex < ImportedObjectsList.Count && currentPage < GetImportEntriesPageSize;
                GetImportEntriesIndex++, currentPage++)
            {
                if (ImportedObjectsList[GetImportEntriesIndex].person != null)
                {
                    csentries.Add(ImportedObjectsList[GetImportEntriesIndex].person.GetCSEntryChange());
                }

                if (ImportedObjectsList[GetImportEntriesIndex].group != null)
                {
                    csentries.Add(ImportedObjectsList[GetImportEntriesIndex].group.GetCSEntryChange());
                }

                if (ImportedObjectsList[GetImportEntriesIndex].unit != null)
                {
                    csentries.Add(ImportedObjectsList[GetImportEntriesIndex].unit.GetCSEntryChange());
                }

                if (ImportedObjectsList[GetImportEntriesIndex].employments != null)
                {
                    csentries.Add(ImportedObjectsList[GetImportEntriesIndex].employments.GetCSEntryChange());
                }

            }

            // Store and return
            importReturnInfo.CSEntries = csentries;
            importReturnInfo.MoreToImport = GetImportEntriesIndex < ImportedObjectsList.Count;
            return importReturnInfo;
        }

        #endregion

        #region Export Methods

        KeyedCollection<string, ConfigParameter> exportConfigParameters;

        private static HttpClient Httpclient;

        public void OpenExportConnection(KeyedCollection<string, ConfigParameter> configParameters, Schema types, OpenExportConnectionRunStep exportRunStep)
        {
            Logger.Log.Info("Export starting");
            exportConfigParameters = configParameters;
            ExportAttributes = new PersonAttributes();
            var handler = new HttpClientHandler { Credentials = VismaGetNetworkCredential(exportConfigParameters) };
            Httpclient = new HttpClient(handler);


        // Start of xDocument
        xmlExport = new XDocument(
                new XDeclaration("1.0", "iso-8859-1", "yes"),
                new XElement("personsXML", new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance")));

            return;
        }

        public PutExportEntriesResults PutExportEntries(IList<CSEntryChange> csentries)
        {
            foreach (CSEntryChange csentry in csentries)
            {
                //if (csentry.DN.Contains("-"))
                //{
                //    ExportAttributes.PersonIdHRM = csentry.DN.ToString().Split('-').GetValue(1).ToString();
                //}
                //else
                //{
                //    ExportAttributes.PersonIdHRM = csentry.DN;
                //}
                
                Logger.Log.InfoFormat("Eksport oppdaget på bruker med ID, {0} av typen {1}", csentry.DN, csentry.ObjectModificationType);

                //String userInfo = VismaGetUserID(exportConfigParameters, ExportAttributes.PersonIdHRM);
                //ExportAttributes.UserId = XDocument.Parse(userInfo).Element("person").Element("authentication").Element("userId").Value;

                if (csentry.DN.Contains("-"))
                {
                    ExportAttributes.UserId = csentry.DN.ToString().Split('-').GetValue(1).ToString();
                }
                else
                {
                    ExportAttributes.UserId = csentry.DN;
                }
                Logger.Log.Info("userId: " + ExportAttributes.UserId);

                switch (csentry.ObjectModificationType)
                {
                    case ObjectModificationType.Delete:
                        break;
                    case ObjectModificationType.Unconfigured:
                        break;
                    case ObjectModificationType.None:
                        break;
                    case ObjectModificationType.Add:
                    case ObjectModificationType.Replace:
                    case ObjectModificationType.Update:
                        if (exportConfigParameters["Export type"].Value == "Simple")
                        {
                            //Adds to url and export data once pr person
                            ExportPrPerson(csentry);
                        }
                        else
                        {
                            // Adds attributes and builds one complete xml file
                            AddPersonAttributes(csentry);
                            BuildExportXML();
                        }
                        break;
                }
            }

            PutExportEntriesResults exportEntriesResults = new PutExportEntriesResults();
            return exportEntriesResults;
        }

        public void CloseExportConnection(CloseExportConnectionRunStep exportRunStep)
        {
            // If advanced export type is selected, a complete xml file is exported
            if (exportConfigParameters["Export type"].Value == "Advanced")
            {
                string _serverportprotocol = exportConfigParameters["Protocol"].Value + "://" + exportConfigParameters["Hostname"].Value + ":" + exportConfigParameters["Port"].Value;
                string exporturl = _serverportprotocol + "/hrm_ws/secure/persons/import";
                //UploadData(xmlExport.ToString(), exporturl);
            }
            Logger.Log.Info("Export finished");
        }
        
        // Simple Export
        private void ExportPrPerson(CSEntryChange csentry)
        {
            string URL = "";
            string _serverportprotocol = exportConfigParameters["Protocol"].Value + "://" + exportConfigParameters["Hostname"].Value + ":" + exportConfigParameters["Port"].Value;
            string exporturl = _serverportprotocol + "/enterprise_ws/secure/user/";
            string userId = ExportAttributes.UserId;
            string value = "";
            string method = "";
            Dictionary<string, List<string>> exportattribs = new Dictionary<string, List<string>>();
            AttributeChange attributeChange;
            IList<ValueChange> valueChanges;

            foreach (var attrib in csentry.ChangedAttributeNames)
            {
                switch (attrib)
                {
                    #region email
                    case SchemaAttributes.email:
                        attributeChange = csentry.AttributeChanges[attrib];
                        valueChanges = attributeChange.ValueChanges;
                        foreach (ValueChange valueChange in valueChanges)
                        {
                            switch (valueChange.ModificationType)
                            {
                                case ValueModificationType.Add:
                                    // Update target system's value.
                                    value = valueChange.Value.ToString();
                                    method = csentry.AttributeChanges[attrib].ModificationType == AttributeModificationType.Delete ? Method.DELETE : Method.PUT;
                                    URL = exporturl + userId + "/email/WORK/" + value;
                                    Logger.Log.Info(URL);
                                    HttpRequest(URL, userId, method, attrib, value);
                                    Logger.Log.InfoFormat("Eksport {0}: {1}", attrib, value);
                                    break;
                            }
                        }
                        break;
                    #endregion

                    #region workphone
                    case SchemaAttributes.workPhone:
                        attributeChange = csentry.AttributeChanges[attrib];
                        valueChanges = attributeChange.ValueChanges;
                        foreach (ValueChange valueChange in valueChanges)
                        {
                            switch (valueChange.ModificationType)
                            {
                                case ValueModificationType.Add:
                                    // Update target system's value.
                                    value = valueChange.Value.ToString();
                                    method = csentry.AttributeChanges[attrib].ModificationType == AttributeModificationType.Delete ? Method.DELETE : Method.PUT;
                                    URL = exporturl + userId + "/phone/WORK/" + value;
                                    Logger.Log.Info(URL);
                                    HttpRequest(URL, userId, method, attrib, value);
                                    Logger.Log.InfoFormat("Eksport {0}: {1}", attrib, value);
                                    break;
                            }
                        }
                        break;
                    #endregion

                    #region workmobile
                    case SchemaAttributes.workMobile:
                        attributeChange = csentry.AttributeChanges[attrib];
                        valueChanges = attributeChange.ValueChanges;
                        foreach (ValueChange valueChange in valueChanges)
                        {
                            switch (valueChange.ModificationType)
                            {
                                case ValueModificationType.Add:
                                    // Update target system's value.
                                    value = valueChange.Value.ToString();
                                    method = csentry.AttributeChanges[attrib].ModificationType == AttributeModificationType.Delete ? Method.DELETE : Method.PUT;
                                    URL = exporturl + userId + "/phone/MOBILE/" + value;
                                    Logger.Log.Info(URL);
                                    HttpRequest(URL, userId, method, attrib, value);
                                    Logger.Log.InfoFormat("Eksport {0}: {1}", attrib, value);
                                    break;
                            }
                        }
                        break;
                    #endregion

                    #region alias
                    // Adds alias (username) to aliases in HRM
                    case SchemaAttributes.alias:
                        
                        foreach (var alias in csentry.AttributeChanges[attrib].ValueChanges)
                        {
                            string uri = exporturl + userId + "/username";
                            value = alias.Value.ToString();
                            method = alias.ModificationType == ValueModificationType.Delete ? Method.DELETE : Method.POST;
                            if (method == Method.POST)
                            {
                                Postdata(uri, attrib, "user=" + value, method);
                                Logger.Log.InfoFormat("Eksport {0}: {1}", attrib, value);
                            }
                            else
                            {
                                URL = exporturl + userId + "/username/" + value;
                                Logger.Log.Info(URL);
                                HttpRequest(URL, userId, method, attrib, value);
                                Logger.Log.InfoFormat("Delete {0} med verdi {1}", attrib, value);
                            }
                        }
                        break;
                    #endregion

                    #region initials
                    case SchemaAttributes.initials:
                        attributeChange = csentry.AttributeChanges[attrib];
                        valueChanges = attributeChange.ValueChanges;
                        foreach (ValueChange valueChange in valueChanges)
                        {
                            switch (valueChange.ModificationType)
                            {
                                case ValueModificationType.Add:
                                    // Update target system's value.
                                    value = valueChange.Value.ToString();
                                    method = csentry.AttributeChanges[attrib].ModificationType == AttributeModificationType.Delete ? Method.DELETE : Method.PUT;
                                    if (method == Method.DELETE)
                                    {
                                        Logger.Log.InfoFormat("{0} er ikke støttet av Visma WS på attributt {1}", method, attrib);
                                    }
                                    else
                                    {
                                        URL = exporturl + userId + "/initials/" + value;
                                        Logger.Log.Info(URL);
                                        HttpRequest(URL, userId, method, attrib, value);
                                        Logger.Log.InfoFormat("Eksport {0}: {1}", attrib, value);
                                    }
                                    break;
                            }
                        }
                        break;
                        #endregion

                    default:
                        break;
                }
            }
            return;
        }

        // Advanced Export
        private void AddPersonAttributes(CSEntryChange csentry)
        {
            if (exportConfigParameters["Prefix personDN med selskapsnummer"].Value == "1")
            {
                ExportAttributes.UserId = csentry.DN.ToString().Split('-').GetValue(1).ToString();
            }
            else
            {
                ExportAttributes.UserId = csentry.DN.ToString();
            }

            // Add new person element to personsXML element in XML
            xmlExport.Element("personsXML").Add(new XElement("person", new XAttribute("externalSystem", "Enterprise"), new XAttribute("personIdHRM", ExportAttributes.PersonIdHRM)));

            AttributeChange attributeChange;
            IList<ValueChange> valueChanges;
            string userId = ExportAttributes.UserId;

            foreach (var attrib in csentry.ChangedAttributeNames)
            {
                switch (attrib)
                {
                    #region personIDHRM
                    case SchemaAttributes.personIdHRM:
                        attributeChange = csentry.AttributeChanges[attrib];
                        valueChanges = attributeChange.ValueChanges;
                        foreach (ValueChange valueChange in valueChanges)
                        {
                            switch (valueChange.ModificationType)
                            {
                                case ValueModificationType.Add:
                                case ValueModificationType.Unconfigured:
                                
                                    // Update target system's value.
                                    ExportAttributes.PersonIdHRM = valueChange.Value.ToString();
                                    Logger.Log.InfoFormat("La til {0}: {1}", attrib, valueChange.Value.ToString());
                                    break;
                            }
                        }
                        break;
                    #endregion

                    #region email
                    case SchemaAttributes.email:
                        attributeChange = csentry.AttributeChanges[attrib];
                        valueChanges = attributeChange.ValueChanges;
                        foreach (ValueChange valueChange in valueChanges)
                        {
                            switch (valueChange.ModificationType)
                            {
                                case ValueModificationType.Add:
                                    // Update target system's value.
                                    ExportAttributes.Email = valueChange.Value.ToString();
                                    //Logger.Log.InfoFormat("Eksport {0}: {1}", attrib, valueChange.Value.ToString());
                                    break;
                            }
                        }
                        break;
                    #endregion

                    #region workphone
                    case SchemaAttributes.workPhone:
                        attributeChange = csentry.AttributeChanges[attrib];
                        valueChanges = attributeChange.ValueChanges;
                        foreach (ValueChange valueChange in valueChanges)
                        {
                            switch (valueChange.ModificationType)
                            {
                                case ValueModificationType.Add:
                                    // Update target system's value.
                                    ExportAttributes.WorkPhone = valueChange.Value.ToString();
                                    Logger.Log.InfoFormat("Eksport {0}: {1}", attrib, valueChange.Value.ToString());
                                    break;
                            }
                        }
                        break;
                    #endregion

                    #region workmobile
                    case SchemaAttributes.workMobile:
                        attributeChange = csentry.AttributeChanges[attrib];
                        valueChanges = attributeChange.ValueChanges;
                        foreach (ValueChange valueChange in valueChanges)
                        {
                            switch (valueChange.ModificationType)
                            {
                                case ValueModificationType.Add:
                                    // Update target system's value.
                                    ExportAttributes.WorkMobile = valueChange.Value.ToString();
                                    Logger.Log.InfoFormat("Eksport {0}: {1}", attrib, valueChange.Value.ToString());
                                    break;
                            }
                        }
                        break;
                    #endregion

                    #region privateMobile
                    case SchemaAttributes.privateMobile:
                        attributeChange = csentry.AttributeChanges[attrib];
                        valueChanges = attributeChange.ValueChanges;
                        foreach (ValueChange valueChange in valueChanges)
                        {
                            switch (valueChange.ModificationType)
                            {
                                case ValueModificationType.Add:
                                    // Update target system's value.
                                    ExportAttributes.PrivateMobile = valueChange.Value.ToString();
                                    Logger.Log.InfoFormat("Eksport {0}: {1}", attrib, valueChange.Value.ToString());
                                    break;
                            }
                        }
                        break;
                    #endregion

                    #region privatePhone
                    case SchemaAttributes.privatePhone:
                        attributeChange = csentry.AttributeChanges[attrib];
                        valueChanges = attributeChange.ValueChanges;
                        foreach (ValueChange valueChange in valueChanges)
                        {
                            switch (valueChange.ModificationType)
                            {
                                case ValueModificationType.Add:
                                    // Update target system's value.
                                    ExportAttributes.PrivatePhone = valueChange.Value.ToString();
                                    Logger.Log.InfoFormat("Eksport {0}: {1}", attrib, valueChange.Value.ToString());
                                    break;
                            }
                        }
                        break;
                    #endregion

                    #region Jobtitle
                    case SchemaAttributes.jobTitle:
                        attributeChange = csentry.AttributeChanges[attrib];
                        valueChanges = attributeChange.ValueChanges;
                        foreach (ValueChange valueChange in valueChanges)
                        {
                            switch (valueChange.ModificationType)
                            {
                                case ValueModificationType.Add:
                                    // Update target system's value.
                                    ExportAttributes.JobTitle = valueChange.Value.ToString();
                                    Logger.Log.InfoFormat("Eksport {0}: {1}", attrib, valueChange.Value.ToString());
                                    break;
                            }
                        }
                        break;
                    #endregion

                    #region alias
                    // Adds alias (username) to aliases in HRM
                    case SchemaAttributes.alias:
                        foreach (var alias in csentry.AttributeChanges[attrib].ValueChanges)
                        {
                            ExportAttributes.Alias.Add(alias.Value.ToString());
                            Logger.Log.InfoFormat("Eksport {0}: {1}", attrib, alias.Value.ToString());
                        }
                        break;
                    #endregion

                    #region initials
                    case SchemaAttributes.initials:
                        attributeChange = csentry.AttributeChanges[attrib];
                        valueChanges = attributeChange.ValueChanges;
                        foreach (ValueChange valueChange in valueChanges)
                        {
                            switch (valueChange.ModificationType)
                            {
                                case ValueModificationType.Add:
                                    // Update target system's value.
                                    string method = csentry.AttributeChanges[attrib].ModificationType == AttributeModificationType.Delete ? Method.DELETE : Method.PUT;
                                    if (method == Method.DELETE)
                                    {
                                        Logger.Log.InfoFormat("{0} er ikke støttet av Visma WS på attributt {1}", method, attrib);
                                    }
                                    else
                                    {
                                        ExportAttributes.Initials = valueChange.Value.ToString();
                                        Logger.Log.InfoFormat("Eksport {0}: {1}", attrib, valueChange.Value.ToString());
                                    }
                                    break;
                            }
                        }
                        break;
                    #endregion

                    default:
                        break;
                }
            }
        }

        private void BuildExportXML()
        {
            xmlExport.Element("personsXML").Add(new XElement("person", new XAttribute("externalSystem", "Enterprise"), new XAttribute("personIdHRM", ExportAttributes.PersonIdHRM)));
            // Builds user specific xElements
            xmlExport.Element("personsXML").Element("person").Add(new XElement("contactInfo",
                ExportAttributes.Email != null ? new XElement("email", ExportAttributes.Email) : new XElement("email", ""),
                ExportAttributes.Email != null ? new XElement("paycheckByEmail", Boolean.TrueString.ToUpper()) : new XElement("paycheckByEmail", Boolean.FalseString.ToUpper()),
                ExportAttributes.WorkPhone != null ? new XElement("workPhone", ExportAttributes.WorkPhone) : new XElement("workPhone", ""),
                ExportAttributes.WorkMobile != null ? new XElement("mobilePhone", ExportAttributes.WorkMobile) : new XElement("mobilePhone", ""),
                ExportAttributes.PrivateMobile != null ? new XElement("privateMobilePhone", ExportAttributes.PrivateMobile) : new XElement("privateMobilePhone", ""),
                ExportAttributes.PrivatePhone != null ? new XElement("privatePhone", ExportAttributes.PrivatePhone) : new XElement("privatePhone", ""),
                ExportAttributes.JobTitle != null ? new XElement("jobTitle", ExportAttributes.JobTitle) : new XElement("jobTitle", "")));
            xmlExport.Element("personsXML").Element("person").Add(new XElement("authentication"));
                xmlExport.Element("personsXML").Element("person").Element("authentication").Add
                (ExportAttributes.Initials != null ? new XElement("initials", ExportAttributes.Initials) : null );

            // Loop trough Alias and add all to authentication element
            foreach (var alias in ExportAttributes.Alias)
            {
                xmlExport.Element("personsXML").Element("person").Element("authentication").Add(new XElement("alias", alias.ToString()));
            }
            
        }

        private void Postdata(string uri, string attrib, string postData, string method)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Credentials = VismaGetNetworkCredential(exportConfigParameters);
            request.Timeout = 3000;
            var data = Encoding.ASCII.GetBytes(postData);
            request.Method = method;           
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            try
            {
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
                var response = (HttpWebResponse)request.GetResponse();
                Logger.Log.InfoFormat("Oppdatert {0}. Svar fra server {1}", attrib, response.StatusCode.ToString());
            }
            catch (Exception message)
            {
                Logger.Log.ErrorFormat("Feil med oppdatering av {0} med feil: {1}", attrib, message);
            }
        }

        public void UploadData(string data, string url)
        {
            // Upload data to uri. Method require multipart/form-data
            string response = null;
            string res = "Error uploading to server";

            try
            {
                UriBuilder wfURI = new UriBuilder(url);
                HttpWebRequest webrequest;
                string sBoundary = "----------" + DateTime.Now.Ticks.ToString("x");
                webrequest = (HttpWebRequest)WebRequest.Create(wfURI.Uri);
                webrequest.ContentType = "multipart/form-data; boundary=" + sBoundary;
                webrequest.Method = Method.POST;
                webrequest.Credentials = VismaGetNetworkCredential(exportConfigParameters);
                webrequest.AllowWriteStreamBuffering = true;

                // Create header
                StringBuilder sbHeader = new StringBuilder();
                sbHeader.Append("--");
                sbHeader.Append(sBoundary);
                sbHeader.Append("\r\n");
                sbHeader.Append("Content-Disposition: form-data; name=\"file\"; filename=\"");
                sbHeader.Append(data);
                sbHeader.Append("\"");
                sbHeader.Append("\r\n");
                sbHeader.Append("Content-Type: application/octet-stream");
                sbHeader.Append("\r\n");
                sbHeader.Append("\r\n");

                string postHeader = sbHeader.ToString();
                byte[] postHeaderBytes = Encoding.UTF8.GetBytes(postHeader);

                // Build the trailing boundary string as a byte array
                // ensuring the boundary appears on a line by itself
                byte[] btBoundary = Encoding.ASCII.GetBytes("\r\n--" + sBoundary + "--\r\n");

                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(data));
                long length = postHeaderBytes.Length + ms.Length + btBoundary.Length;
                webrequest.ContentLength = length;

                Stream requestStream = webrequest.GetRequestStream();

                // Write out our post header
                requestStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);

                // Write out the file contents
                byte[] buffer = new byte[checked((uint)Math.Min(4096, (int)ms.Length))];
                int bytesRead = 0;
                while ((bytesRead = ms.Read(buffer, 0, buffer.Length)) != 0)
                    requestStream.Write(buffer, 0, bytesRead);

                // Write out the trailing boundary
                requestStream.Write(btBoundary, 0, btBoundary.Length);
                ms.Close();
                requestStream.Close();

                //HttpWebResponse wres = (HttpWebResponse)webrequest.GetResponse();

                var sr = new StreamReader(webrequest.GetResponse().GetResponseStream());
                response = sr.ReadToEnd();

                //if(wres.StatusCode == HttpStatusCode.OK)
                //{
                //    res = "Upload OK";
                //}
                //else
                //{
                //    res = "Error: " + wres.StatusCode.ToString() + ": " + wres.StatusDescription;
                //}

                if (response.Contains("200"))
                {
                    res = "Upload OK";
                }
                else
                {
                    res = "Error uploading to server: " + response;
                }
            }
            catch (WebException err)
            {
                Logger.Log.Error(err.Message);
            }
            finally
            {
                Logger.Log.Info("Result: " + res);
            }
        }

        public void HttpRequest(string url, string userId, string method, string attrib, string value)
        {
            HttpWebRequest webreq = (HttpWebRequest)WebRequest.Create(url);
            webreq.Method = method;
            webreq.Credentials = VismaGetNetworkCredential(exportConfigParameters);
            webreq.ContentType = "text/plain";
            webreq.Timeout = 3000;
            try
            {
                HttpWebResponse response = (HttpWebResponse)webreq.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Logger.Log.InfoFormat("Export: (HTTP: {0}. {1} på userId {2} er utført. Berørt attributt er {3} med verdien {4}", response.StatusCode, method, userId, attrib, value);
                }
                else
                {
                    Logger.Log.ErrorFormat("Export Feilet: (HTTP: {0}. {1} feilet på userId {2}, på attributt {3} med verdi {4}", response.StatusCode, method, userId, attrib, value);
                }
                response.Close();
            }
            catch (Exception httperror)
            {
                Logger.Log.Error("Export:" + httperror.Message);
            }
            webreq.Abort();
        }

        #endregion
    }
}

