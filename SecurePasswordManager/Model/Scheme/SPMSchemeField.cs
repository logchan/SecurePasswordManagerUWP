using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

using SecurePasswordManager.Core;

namespace SecurePasswordManager.Model.Scheme
{
    [XmlRoot("SPMSchemeField", IsNullable = false)]
    public class SPMSchemeField
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string description;

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        private SPMSchemeSaltingType saltingType;

        public SPMSchemeSaltingType SaltingType
        {
            get { return saltingType; }
            set { saltingType = value; }
        }

        [XmlIgnore, MenuFlyoutIndicator]
        public bool isActual = true;

        public SPMSchemeField()
        {
            name = "Default Field";
            description = "No description.";
            saltingType = SPMSchemeSaltingType.ALWAYS;
        }

        public SPMSchemeField(SPMSchemeField o)
        {
            name = o.name;
            description = o.description;
            saltingType = o.saltingType;
        }
    }
}
