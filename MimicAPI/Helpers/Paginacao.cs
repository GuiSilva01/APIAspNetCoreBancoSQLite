﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace MimicAPI.Helpers
{
    public class Paginacao
    {
        public int  NumeroPagina { get; set; }

        public int RegistrosPorPagina { get; set; }

        public int TotalRegistros { get; set; }

        public int TotalPagina { get; set; }
    }
}
