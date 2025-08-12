  using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROYECTWEVENTS.CustClasses;
public class RegistrardatosUsuario
{
    public string Usuario { get; set; }
    public string Correo { get; set; }
    public string Contrasena { get; set; }
    public string Telefono { get; set; }
    public string Nombres { get; set; }
    public string Apellidos { get; set; }
    public string Pais { get; set; }
    public string Estado { get; set; }
    public string Municipio { get; set; }
    public string CodigoPostal { get; set; }
    public decimal Lat { get; set; }   
    public decimal Lon { get; set; }
    public byte[] CURP { get; set; } = Array.Empty<byte>();
    public byte[] IneFront { get; set; } = Array.Empty<byte>();
    public byte[] IneBack { get; set; } = Array.Empty<byte>();
    public byte[] Selfie { get; set; } = Array.Empty<byte>();
    public bool FaceMatch { get; set; }      
}
public class ArchivoItem
{
    public int FileId { get; set; }
    public string Filename { get; set; }
    public string FileUrl => $"https://clouddatacancun.com/registrousersyarchs.php?token_operacion=3&file_id={FileId}";

}
public class LocationData
{
    public string Country { get; set; }
    public List<StateData> States { get; set; }
}

public class StateData
{
    public string State { get; set; }
    public List<string> Municipalities { get; set; }
}


