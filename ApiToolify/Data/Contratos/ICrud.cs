namespace ProyectoDSWToolify.Data.Contratos
{
    public interface ICrud <T> where T : class
    {
        /*CRUDS GENERALES*/
        List<T> ListaCompleta();    
        T ObtenerId(string tipo,int id);
        T Registrar(string tipo,T entidad);
        T Actualizar(string tipo,T entidad);
        int Eliminar(string tipo,int id);
    }
}
