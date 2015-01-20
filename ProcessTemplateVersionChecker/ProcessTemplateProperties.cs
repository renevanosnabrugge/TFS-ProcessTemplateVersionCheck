using Microsoft.TeamFoundation.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessTemplateVersionChecker
{
    public class ProcessTemplateProperties
    {
        private string _processtemplate;

        public string ProcessTemplate
        {
            get { return _processtemplate; }
            set { _processtemplate = value; }
        }


        private string _createVersion;

        public string CreateVersion
        {
            get
            {
                if (string.IsNullOrEmpty(_createVersion))
                {
                    _createVersion = "Not Set";
                }
                return _createVersion;
            }
            set { _createVersion = value; }
        }

        private string _currentVersion;

        public string CurrentVersion
        {
            get
            {
                if (string.IsNullOrEmpty(_currentVersion))
                {
                    _currentVersion = "Not Set";
                }
                return _currentVersion;
            }
            set { _currentVersion = value; }
        }

        public ProjectInfo CurrentProject { get; set; }

        public const string CREATEVERSIONSTRING = "Create Version";
        public const string CURRENTVERSIONSTRING = "Current Version";
        public const string PROCESSTEMPLATESTRING = "Process Template";

   
    }
}
