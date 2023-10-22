using IBGE.API.Domain;
using IBGE.API.Extensions;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Data;

namespace IBGE.API.Context;

public class DbContext
{
    private static readonly string _connectionString = "";

    private static string? _msgError;
    private static object? _return;

    public static string GetStringConnection()
    {
        return _connectionString;
    }

    public static string? GetMsgError()
    {
        return _msgError;
    }

    public static object? GetReturn()
    {
        return _return;
    }

    public static T? GetReturn<T>()
    {
        return JsonConvert.DeserializeObject<T>(GetReturn().ToString());
    }

    private async static Task<MySqlConnection?> ConnectAsync()
    {
        try
        {
            var _conn = new MySqlConnection(GetStringConnection());
            await _conn.OpenAsync();
            return _conn;
        }
        catch (Exception ex)
        {
            _msgError = ex.Message;
            return null;
        }
    }

    private async static Task<bool> DisconnectAsync(MySqlConnection con)
    {
        try
        {
            if (con.State == ConnectionState.Open)
            {
                await con.CloseAsync();
                await con.DisposeAsync();
            }
            return true;
        }
        catch (Exception ex)
        {
            await con.DisposeAsync();
            _msgError = ex.Message;
            return false;
        }

    }

    public async static Task<DataTable?> SelectAsync(Locality entity, Operating op)
    {
        var con = await ConnectAsync();
        try
        {
            var dt = new DataTable();
            var cmd = new MySqlCommand("stp_select_locality", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("p_op", op.ToString());
            cmd.Parameters.AddWithValue("p_filter", entity.GetFilter());
            var data = new MySqlDataAdapter(cmd);
            await data.FillAsync(dt);
            data.Dispose();

            await cmd.DisposeAsync();
            return dt;
        }
        catch (Exception ex)
        {
            _msgError = ex.Message;
            return null;
        }
        finally
        {
            if (con is not null)
                await DisconnectAsync(con);
        }
    }

    public static async Task<bool> SaveAsync(Locality entity, Operating op)
    {
        var con = await ConnectAsync();
        try
        {
            var cmd = new MySqlCommand("stp_locality", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("p_op", op.ToString());
            cmd.Parameters.AddWithValue("p_id", entity.Id).Direction = ParameterDirection.InputOutput;
            cmd.Parameters.AddWithValue("p_city", entity.City);
            cmd.Parameters.AddWithValue("p_state", entity.State.ToString());
            _msgError = (string)cmd.ExecuteScalar();
            _return = cmd.Parameters["?p_id"].Value;
            cmd.Dispose();
            return _msgError is null;
        }
        catch (Exception ex)
        {
            _msgError = ex.Message;
            return false;
        }
        finally { if (con is not null) await DisconnectAsync(con); }
    }
}
