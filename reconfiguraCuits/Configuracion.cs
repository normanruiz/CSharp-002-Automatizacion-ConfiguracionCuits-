using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace reconfiguraCuits
{
    public class Configuracion
    {

        public String rutaScripts { get; set; }

        public String rutaCuits { get; set; }

        public String rutaTR { get; set; }

        public String token { get; set; }

        public String canalSlack { get; set; }

        public String servicio { get; set; }

        public XDocument archivoConfiguracion;

        public Configuracion(String rutaConfiguracion)
        {
            StringBuilder result = new StringBuilder();
            archivoConfiguracion = XDocument.Load(rutaConfiguracion);
            var configuraciones = from configuracion in archivoConfiguracion.Descendants("configuracion")
                                  select new
                                  {
                                      Header = configuracion.Attribute("value").Value,
                                      Children = configuracion.Descendants("parametro")
                                  };

            foreach (var configuracion in configuraciones)
            {
                foreach (var parametro in configuracion.Children)
                {
                    if (configuracion.Header == "rutaScripts")
                    {
                        rutaScripts = parametro.Attribute("value").Value;
                    }
                    if (configuracion.Header == "rutaCuits")
                    {
                        rutaCuits = parametro.Attribute("value").Value;
                    }
                    if (configuracion.Header == "rutaTR")
                    {
                        rutaTR = parametro.Attribute("value").Value;
                    }
                    if (configuracion.Header == "token")
                    {
                        token = parametro.Attribute("value").Value;
                    }
                    if (configuracion.Header == "canalSlack")
                    {
                        canalSlack = parametro.Attribute("value").Value;
                    }
                    if (configuracion.Header == "servicio")
                    {
                        servicio = parametro.Attribute("value").Value;
                    }
                }
            }
        }
    }
}
