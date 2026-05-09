namespace ColegioSanJose.ViewModels
{
    public class ExpedienteAlumnoViewModel
    {
        public int AlumnoId { get; set; }

        public string NombreCompleto { get; set; } = string.Empty;

        public string Grado { get; set; } = string.Empty;

        public decimal Promedio { get; set; }

        public int CantidadMaterias { get; set; }

        public List<ExpedienteDetalleViewModel> Expedientes { get; set; } = new List<ExpedienteDetalleViewModel>();
    }

    public class ExpedienteDetalleViewModel
    {
        public int ExpedienteId { get; set; }

        public string Materia { get; set; } = string.Empty;

        public string Docente { get; set; } = string.Empty;

        public decimal NotaFinal { get; set; }

        public string? Observaciones { get; set; }
    }
}