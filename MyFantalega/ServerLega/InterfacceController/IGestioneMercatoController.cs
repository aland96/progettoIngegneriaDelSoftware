﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerLega.Dominio;

namespace ServerLega.InterfacceController
{
    interface IGestioneMercatoController
    {
        Boolean ChiudiMercato(Mercato mercato);
    }
}
