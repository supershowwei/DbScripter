﻿namespace DbScripter
{
    internal class Arguments
    {
        public string ConnectionString { get; set; }

        public string Database { get; set; }

        public string[] Tables { get; set; }

        public string[] Views { get; set; }

        public string[] StoredProcedures { get; set; }

        public string Type { get; set; }

        public string Output { get; set; }
    }
}