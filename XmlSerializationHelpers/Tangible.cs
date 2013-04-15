﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TangibleAnchoring.XmlSerializationHelpers
{
    public class Tangible
    {
        [XmlElement]
        public string Type;

        [XmlElement]
        public string TagId;

        [XmlElement]
        public string Name;

        public Facet[] Rotation;

    }
}
