namespace ProyectoDSWToolify.Data.Contratos
{
    public interface ICrud <T> where T : class
    {
        /*CRUDS GENERALES*/
        List<T> ListaCompleta();
        T ObtenerId(string tipo,int id);
        int Registrar(string tipo,T entidad);
        bool Actualizar(string tipo,T entidad);
        bool Eliminar(string tipo,int id);
    }
}
