using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eternity1._0.Models.Items
{
    public class ItemUbicaciones
    {
        public string itemCode { get; set; }
        public string itemName { get; set; }
        public string CodeBars { get; set; }
        public decimal SWeight1 { get; set; }
        public int ItmsGrpCod { get; set; }
        public string U_codigo { get; set; }
        public List<UbicacionesItems> ubicacionesItems { get; set; }

        public ItemUbicaciones(string itemCode, string itemName, string codeBars, decimal sWeight1, int itmsGrpCod, string u_codigo,List<UbicacionesItems> ubicaciones)
        {
            this.itemCode = itemCode;
            this.itemName = itemName;
            CodeBars = codeBars;
            SWeight1 = sWeight1;
            ItmsGrpCod = itmsGrpCod;
            U_codigo = u_codigo;
            this.ubicacionesItems = ubicaciones;
        }
    }
}
