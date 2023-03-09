using Eternity1._0.Models;
using Eternity1._0.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System.Data;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eternity1._0.Models.Items;

namespace Eternity1._0.Controllers
{


    [ApiController]
    public class ItemController : Controller
    {
        string sessionId;
        private readonly string cadenaSQL;


        LoginService service = new LoginService();
        ItemUbicacionesService itemService = new ItemUbicacionesService();

        public ItemController(IConfiguration config)
        {
            cadenaSQL = config.GetConnectionString("CadenaSQL");
        }

        [Route("ananda/eternity/ItemTales")]
        [HttpGet]
        public IActionResult findItemByItemCodeTales(string itemCode)
        {
            var cliente = new RestClient("https://199.89.53.35:50000/b1s/v1/Items" + "('" + itemCode + "')");
            cliente.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Cookie", "B1SESSION=" + service.LoginSAP());
            var body = @"";
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            IRestResponse response = cliente.Execute(request);
            var resultado = JsonConvert.DeserializeObject<Item>(response.Content, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            ItemUbicaciones item = new ItemUbicaciones(resultado.ItemCode,resultado.ItemName,resultado.BarCode,resultado.SalesUnitWeight,resultado.ItemsGroupCode,resultado.U_codigo,itemService.listarUbicaciones(itemCode));
            
            if (item.itemCode != null)
            {
                List<UbicacionesItems> lista = new List<UbicacionesItems>();
                List<UbicacionesItems> listaUbis = new List<UbicacionesItems>();

                try
                {
                    using (var conexion = new SqlConnection(cadenaSQL))
                    {
                        conexion.Open();
                        var cmd = new SqlCommand("sp_traerUbicaciones", conexion);
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (var rd = cmd.ExecuteReader())
                        {

                            while (rd.Read())
                            {
                                decimal stockUbicacion;
                                decimal totalStockAlmacen;

                                if (rd["stockUbicacion"] != DBNull.Value)
                                {
                                    stockUbicacion = Convert.ToDecimal(rd["stockUbicacion"]);
                                }
                                else
                                {
                                    stockUbicacion = 0m;
                                }
                                if (rd["TotalStockAlmacen"] != DBNull.Value)
                                {
                                    totalStockAlmacen = Convert.ToDecimal(rd["TotalStockAlmacen"]);
                                }
                                else
                                {
                                    totalStockAlmacen = 0m;
                                }
                                lista.Add(new UbicacionesItems
                                {
                                    AbsEntry = rd["AbsEntry"].ToString(),
                                    BinCode = rd["Bincode"].ToString(),
                                    ItemCode = rd["ItemCode"].ToString(),
                                    ItemName = rd["ItemName"].ToString(),
                                    WhsCode = rd["WhsCode"].ToString(),
                                    StockUbicacion = stockUbicacion,
                                    TotalStockAlmacen = totalStockAlmacen
                                });
                            }

                        }

                    }
                    listaUbis = lista.Where(item => item.ItemCode == itemCode).ToList();
                    item.ubicacionesItems = listaUbis;
                    return StatusCode(StatusCodes.Status200OK,item);

                }
                catch (Exception error)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, item);
                }
            }
            else
            {
                item.itemCode = "0"; ;
                return StatusCode(StatusCodes.Status200OK, item);
            }

        }


        [HttpGet]
        [Route("Item")]
        public IActionResult traerItemByItemCode(string itemCode)
        {
            var cliente = new RestClient("https://199.89.53.35:50000/b1s/v1/Items" + "('" + itemCode + "')");
            cliente.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Cookie", "B1SESSION=" + service.LoginSAP());
            var body = @"";
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            IRestResponse response = cliente.Execute(request);
            var resultado = JsonConvert.DeserializeObject<Item>(response.Content, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            Item item = new Item(resultado.ItemCode,resultado.ItemName);
            if (item.ItemCode != null)
            {
                return StatusCode(StatusCodes.Status200OK, item);
            }
            else
            {
                item.ItemCode = "0"; ;
                item.ItemName = "";
                item.NCMCode = "0";
                item.Properties4 = "";
                return StatusCode(StatusCodes.Status200OK, item);
            }
        }
    }
    }
