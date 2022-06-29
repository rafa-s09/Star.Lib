using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Star.Lib.EntityControls;

/// <summary>
/// Adiciona uma camada de abstração no topo da camada de consulta e ajuda a eliminar a lógica duplicada na implementação do seu código de consulta no modelo de entidade.
/// </summary>
/// <typeparam name="TEntity">O tipo de classe da entidade de destino.</typeparam>
public class Repository<TEntity> : IDisposable where TEntity : class
{
    #region Constructor
    /// <summary>
    /// DbContext representa uma sessão com o banco de dados que pode ser usada para consultar e salvar instâncias de suas entidades em um banco de dados
    /// </summary>
    private DbContext? Context;

    /// <summary>
    /// Construtor de classe
    /// </summary>
    /// <param name="dbContext"><b>dbContext</b> representa uma sessão com o banco de dados que pode ser usada para consultar e salvar instâncias de suas entidades em um banco de dados</param>
    public Repository(DbContext dbContext) => Context = dbContext;
    #endregion

    #region Disposable
    /// <summary>
    /// Desconstrutor classe
    /// </summary>  
    ~Repository() => Dispose();

    /// <summary>
    /// Descartar recursos não gerenciados e suprimir o check-out
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Implementação protegida do Dispose padrão
    /// </summary>
    /// <param name="disposing">Descarta o valor armazenado [Default: false]</param>
    protected virtual void Dispose(bool disposing = false)
    {
        /// Objetos gerenciados
        if (disposing)
            Context = null;
    }
    #endregion

    #region Sync
    /// <summary>
    /// Insire o novo registro na tabela
    /// </summary>
    /// <param name="entity">Entidade</param>
    public void Insert(TEntity entity)
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        Context.Set<TEntity>().Add(entity);
        Context.SaveChanges();
    }

    /// <summary>
    /// Insire um lista de novos registros na tabela
    /// </summary>
    /// <param name="entities">Lista de entidades</param>
    public void BatchInsert(List<TEntity> entities)
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        foreach (TEntity entity in entities)
        {
            Context.Set<TEntity>().Add(entity);
            Context.SaveChanges();
        }

    }

    /// <summary>
    /// Atualiza o registro na tabela
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>Entity</returns>
    public void Update(TEntity entity)
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        Context.Entry(entity).State = EntityState.Modified;
        Context.SaveChanges();
    }

    /// <summary>
    /// Atualiza os registros na tabela
    /// </summary>
    /// <param name="entities">Lista de entidades</param>
    /// <returns>Lista de entidades</returns>
    public void BatchUpdate(List<TEntity> entities)
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        foreach (TEntity entity in entities)
        {
            Context.Entry(entity).State = EntityState.Modified;
            Context.SaveChanges();
        }
    }

    /// <summary>
    /// Remove o registro da tabela atravez do Id
    /// <i>Nota: esse comando é não destrutivo e retorna a entidade de entrada na saida</i>
    /// </summary>
    /// <param name="id">Id</param>
    /// <returns>Entidade</returns>
    public TEntity? DeleteById(params object[] id)
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        TEntity? obj = Context.Set<TEntity>().Find(id);
        if (obj is not null)
        {
            Context.Entry(obj).State = EntityState.Deleted;
            Context.Set<TEntity>().Remove(obj);
            Context.SaveChanges();
            return obj;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Remove o registro da tabela
    /// <i>Nota: esse comando é não destrutivo e retorna a entidade de entrada na saida</i>
    /// </summary>
    /// <param name="entity">Entidade</param>
    /// <returns>Entidade</returns>
    public TEntity? Delete(TEntity entity)
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        Context.Entry(entity).State = EntityState.Deleted;
        Context.Set<TEntity>().Remove(entity);
        Context.SaveChanges();
        return entity;
    }

    /// <summary>
    /// Remove a lista de registros da tabela
    /// <i>Nota: esse comando é não destrutivo e retorna as entidades de entrada na saida</i>
    /// </summary>
    /// <param name="entities">Lista de entidades</param>
    /// <returns>Lista de entidades</returns>
    public List<TEntity> BatchDelete(List<TEntity> entities)
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        foreach (TEntity entity in entities)
        {
            Context.Entry(entity).State = EntityState.Deleted;
            Context.Set<TEntity>().Remove(entity);
            Context.SaveChanges();
        }
        return entities;
    }

    /// <summary>
    /// Realiza a busca de registros na tabela
    /// </summary>
    /// <param name="expression">Expressão Lambda/param>
    /// <returns>Lista de entidades como IEnumerable</returns>
    public IEnumerable<TEntity> Search(Expression<Func<TEntity, bool>> expression)
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        return Context.Set<TEntity>().Where(expression ?? (x => true)).ToList();
    }

    /// <summary>
    /// Realiza a busca de registros na tabela
    /// </summary>
    /// <param name="expression">Expressão Lambda/param>
    /// <returns>Lista de entidades como IQueryable</returns>
    public IQueryable<TEntity> QueryableSearch(Expression<Func<TEntity, bool>> expression)
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        return Context.Set<TEntity>().Where(expression ?? (x => true));
    }

    /// <summary>
    /// Realiza a pesquisa de registros na tabela e ordena o resultado conforme
    /// </summary>
    /// <param name="expression">Expressão Lambda/param>
    /// <param name="sortExpression">Expressão Lambda/param>
    /// <param name="ascendant">Ordem crescente se true [default: true]</param>
    /// <returns>Lista de entidades como IEnumerable</returns>
    public IEnumerable<TEntity> SortedSearch(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, bool>> sortExpression, bool ascendant = true)
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        if (ascendant)
            return Context.Set<TEntity>().Where(expression ?? (x => true)).OrderBy(sortExpression ?? (x => true)).ToList();
        else
            return Context.Set<TEntity>().Where(expression ?? (x => true)).OrderByDescending(sortExpression ?? (x => true)).ToList();
    }

    /// <summary>
    /// Realiza a pesquisa de registros na tabela e ordena o resultado conforme
    /// </summary>
    /// <param name="expression">Expressão Lambda/param>
    /// <param name="sortExpression">Expressão Lambda/param>
    /// <param name="ascendant">Ordem crescente se true [default: true]</param>
    /// <returns>Lista de entidades como IQueryable</returns>
    public IQueryable<TEntity> SortedQueryableSearch(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, bool>> sortExpression, bool ascendant = true)
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        if (ascendant)
            return Context.Set<TEntity>().Where(expression ?? (x => true)).OrderBy(sortExpression ?? (x => true));
        else
            return Context.Set<TEntity>().Where(expression ?? (x => true)).OrderByDescending(sortExpression ?? (x => true));
    }

    /// <summary>
    /// Retorna todos os registros <br />
    /// <i>Nota: dependendo da quantidade de registros é possivel ocorrer problemas de performace ou tavamentos. (Tente usar o <b>GetThousand</b>, ao invez desse comando)</i>
    /// </summary>
    /// <returns>Lista de entidades ou nulo IEnumerable</returns>
    public IEnumerable<TEntity> GetAll()
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        return Context.Set<TEntity>().ToList();
    }

    /// <summary>
    /// Retorna os primeiros 1000 registros
    /// </summary>
    /// <returns>Lista de entidades ou nulo IEnumerable</returns>
    public IEnumerable<TEntity> GetThousand()
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        return Context.Set<TEntity>().Take(1000).ToList();
    }

    /// <summary>
    /// Retorna o registro da tabela em que o id foi inserido
    /// </summary>
    /// <param name="id">Id</param>
    /// <returns>Entidade ou nulo</returns>
    public TEntity? GetById(params object[] id)
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        return Context.Set<TEntity>().Find(id);
    }

    /// <summary>
    /// Retorna o primeiro registro da tabela
    /// </summary>
    /// <returns>Primeira entidade ou nulo</returns>
    public TEntity? GetFirst()
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        return Context.Set<TEntity>().FirstOrDefault();
    }

    /// <summary>
    /// Retorna o primeiro registro da tabela
    /// </summary>
    /// <param name="expression">Expressão Lambda/param>
    /// <returns>Primeira entidade ou nulo</returns>
    public TEntity? GetFirst(Expression<Func<TEntity, bool>> expression)
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        return Context.Set<TEntity>().FirstOrDefault(expression ?? (x => true));
    }

    /// <summary>
    /// Retorna o último registro da tabela
    /// </summary>
    /// <returns>Última entidade ou nulo</returns>
    public TEntity? GetLast()
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        return Context.Set<TEntity>().LastOrDefault();
    }

    /// <summary>
    /// Retorna o último registro da tabela
    /// </summary>
    /// <param name="expression">Expressão Lambda/param>
    /// <returns>Última entidade ou nulo</returns>
    public TEntity? GetLast(Expression<Func<TEntity, bool>> expression)
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        return Context.Set<TEntity>().LastOrDefault(expression ?? (x => true));
    }

    /// <summary>
    /// Realizar busca de dados na tabela e retornar a quantidade desejada, pulado um determinado valor inicial
    /// </summary>
    /// <param name="expression">Expressão Lambda/param>
    /// <param name="skip">Valor ignorado</param>
    /// <param name="amount">Quantidade desejada</param>
    /// <returns>Lista de entidades como IEnumerable</returns>
    public IEnumerable<TEntity> GetSome(Expression<Func<TEntity, bool>> expression, int skip, int amount)
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        return Context.Set<TEntity>().Where(expression ?? (x => true)).Skip(skip).Take(amount).ToList();
    }

    /// <summary>
    /// Realiza pesquisa de dados na tabela e pula um determinado valor inicial
    /// </summary>
    /// <param name="expression">Expressão Lambda/param>
    /// <param name="skip">Valor ignorado</param>
    /// <returns>Lista de entidades como IEnumerable</returns>
    public IEnumerable<TEntity> Skip(Expression<Func<TEntity, bool>> expression, int skip)
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        return Context.Set<TEntity>().Where(expression ?? (x => true)).Skip(skip).ToList();
    }

    /// <summary>
    /// Realizar busca de dados na tabela e retornar a quantidade desejada
    /// </summary>
    /// <param name="expression">Expressão Lambda/param>
    /// <param name="amount">Quantidade desejada</param>
    /// <returns>Lista de entidades como IEnumerable</returns>
    public IEnumerable<TEntity> Take(Expression<Func<TEntity, bool>> expression, int amount)
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        return Context.Set<TEntity>().Where(expression ?? (x => true)).Take(amount).ToList();
    }

    /// <summary>
    /// Conta a quantidade de registro na tabela
    /// </summary>
    /// <returns>Total</returns>
    public int Count()
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        return Context.Set<TEntity>().Count();
    }

    /// <summary>
    /// Conta a quantidade de registro na tabela
    /// </summary>
    /// <param name="expression">Expressão Lambda/param>
    /// <returns>Total</returns>
    public int Count(Expression<Func<TEntity, bool>> expression)
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        return Context.Set<TEntity>().Count(expression ?? (x => true));
    }

    /// <summary>
    /// Verifica se o registro existe <br/>
    /// <i>Nota: se a expressão retornar mais de um resultado, será considerada existente</i>
    /// </summary>
    /// <param name="expression">expressão Lambda</param>
    /// <returns>Se existir, retorna true</returns>
    public bool Exist(Expression<Func<TEntity, bool>> expression)
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        TEntity? entity = Context.Set<TEntity>().FirstOrDefault(expression ?? (x => true));
        if (entity != null)
            return true;
        else
            return false;
    }

    /// <summary>
    /// Verifica se o registro existe
    /// </summary>
    /// <param name="id">Id do registro</param>
    /// <returns>Se existir, retorna true</returns>
    public bool Exist(params object[] id)
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        TEntity? entity = Context.Set<TEntity>().Find(id);
        if (entity != null)
            return true;
        else
            return false;
    }
    #endregion

    #region Async
    /// <summary>
    /// Insire o novo registro na tabela
    /// </summary>
    /// <param name="entity">Entidade</param>
    public async Task InsertAsync(TEntity entity)
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        await Context.Set<TEntity>().AddAsync(entity);
        await Context.SaveChangesAsync();
    }

    /// <summary>
    /// Insire um lista de novos registros na tabela
    /// </summary>
    /// <param name="entities">Lista de entidades</param>
    public async Task BatchInsertAsync(List<TEntity> entities)
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        foreach (TEntity entity in entities)
        {
            await Context.Set<TEntity>().AddAsync(entity);
            await Context.SaveChangesAsync();
        }

    }

    /// <summary>
    /// Atualiza o registro na tabela
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>Entity</returns>
    public async Task UpdateAsync(TEntity entity)
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        Context.Entry(entity).State = EntityState.Modified;
        await Context.SaveChangesAsync();
    }

    /// <summary>
    /// Atualiza os registros na tabela
    /// </summary>
    /// <param name="entities">Lista de entidades</param>
    /// <returns>Lista de entidades</returns>
    public async Task BatchUpdateAsync(List<TEntity> entities)
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        foreach (TEntity entity in entities)
        {
            Context.Entry(entity).State = EntityState.Modified;
            await Context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Remove o registro da tabela atravez do Id
    /// <i>Nota: esse comando é não destrutivo e retorna a entidade de entrada na saida</i>
    /// </summary>
    /// <param name="id">Id</param>
    /// <returns>Entidade</returns>
    public async Task<TEntity?> DeleteByIdAsync(params object[] id)
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        TEntity? obj = Context.Set<TEntity>().Find(id);
        if (obj is not null)
        {
            Context.Entry(obj).State = EntityState.Deleted;
            Context.Set<TEntity>().Remove(obj);
            await Context.SaveChangesAsync();
            return obj;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Remove o registro da tabela
    /// <i>Nota: esse comando é não destrutivo e retorna a entidade de entrada na saida</i>
    /// </summary>
    /// <param name="entity">Entidade</param>
    /// <returns>Entidade</returns>
    public async Task<TEntity?> DeleteAsync(TEntity entity)
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        Context.Entry(entity).State = EntityState.Deleted;
        Context.Set<TEntity>().Remove(entity);
        await Context.SaveChangesAsync();
        return entity;
    }

    /// <summary>
    /// Remove a lista de registros da tabela
    /// <i>Nota: esse comando é não destrutivo e retorna as entidades de entrada na saida</i>
    /// </summary>
    /// <param name="entities">Lista de entidades</param>
    /// <returns>Lista de entidades</returns>
    public async Task<List<TEntity>> BatchDeleteAsync(List<TEntity> entities)
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        foreach (TEntity entity in entities)
        {
            Context.Entry(entity).State = EntityState.Deleted;
            Context.Set<TEntity>().Remove(entity);
            await Context.SaveChangesAsync();
        }
        return entities;
    }

    /// <summary>
    /// Realiza a busca de registros na tabela
    /// </summary>
    /// <param name="expression">Expressão Lambda/param>
    /// <returns>Lista de entidades como IEnumerable</returns>
    public async Task<IEnumerable<TEntity>> SearchAsync(Expression<Func<TEntity, bool>> expression)
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        List<TEntity> entities = await Context.Set<TEntity>().Where(expression ?? (x => true)).ToListAsync();
        return entities;
    }

    /// <summary>
    /// Realiza a busca de registros na tabela
    /// </summary>
    /// <param name="expression">Expressão Lambda/param>
    /// <returns>Lista de entidades como IQueryable</returns>
    public async Task<IQueryable<TEntity>> QueryableSearchAsync(Expression<Func<TEntity, bool>> expression)
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        IQueryable<TEntity> entities = Context.Set<TEntity>().Where(expression ?? (x => true));
        await Task.CompletedTask;
        return entities;
    }

    /// <summary>
    /// Realiza a pesquisa de registros na tabela e ordena o resultado conforme
    /// </summary>
    /// <param name="expression">Expressão Lambda/param>
    /// <param name="sortExpression">Expressão Lambda/param>
    /// <param name="ascendant">Ordem crescente se true [default: true]</param>
    /// <returns>Lista de entidades como IEnumerable</returns>
    public async Task<IEnumerable<TEntity>> SortedSearchAsync(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, bool>> sortExpression, bool ascendant = true)
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        List<TEntity> entities;

        if (ascendant)
            entities = await Context.Set<TEntity>().Where(expression ?? (x => true)).OrderBy(sortExpression ?? (x => true)).ToListAsync();
        else
            entities = await Context.Set<TEntity>().Where(expression ?? (x => true)).OrderByDescending(sortExpression ?? (x => true)).ToListAsync();

        return entities;
    }

    /// <summary>
    /// Realiza a pesquisa de registros na tabela e ordena o resultado conforme
    /// </summary>
    /// <param name="expression">Expressão Lambda/param>
    /// <param name="sortExpression">Expressão Lambda/param>
    /// <param name="ascendant">Ordem crescente se true [default: true]</param>
    /// <returns>Lista de entidades como IQueryable</returns>
    public async Task<IQueryable<TEntity>> SortedQueryableSearchAsync(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, bool>> sortExpression, bool ascendant = true)
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        IQueryable<TEntity> entities;

        if (ascendant)        
            entities = Context.Set<TEntity>().Where(expression ?? (x => true)).OrderBy(sortExpression ?? (x => true));        
        else
            entities = Context.Set<TEntity>().Where(expression ?? (x => true)).OrderByDescending(sortExpression ?? (x => true));

        await Task.CompletedTask;
        return entities;
    }

    /// <summary>
    /// Retorna todos os registros <br />
    /// <i>Nota: dependendo da quantidade de registros é possivel ocorrer problemas de performace ou tavamentos. (Tente usar o <b>GetThousand</b>, ao invez desse comando)</i>
    /// </summary>
    /// <returns>Lista de entidades ou nulo IEnumerable</returns>
    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        return await Context.Set<TEntity>().ToListAsync();
    }

    /// <summary>
    /// Retorna os primeiros 1000 registros
    /// </summary>
    /// <returns>Lista de entidades ou nulo IEnumerable</returns>
    public async Task<IEnumerable<TEntity>> GetThousandAsync()
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        return await Context.Set<TEntity>().Take(1000).ToListAsync();
    }

    /// <summary>
    /// Retorna o registro da tabela em que o id foi inserido
    /// </summary>
    /// <param name="id">Id</param>
    /// <returns>Entidade ou nulo</returns>
    public async Task<TEntity?> GetByIdAsync(params object[] id)
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        return await Context.Set<TEntity>().FindAsync(id);
    }

    /// <summary>
    /// Retorna o primeiro registro da tabela
    /// </summary>
    /// <returns>Primeira entidade ou nulo</returns>
    public async Task<TEntity?> GetFirstAsync()
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        return await Context.Set<TEntity>().FirstOrDefaultAsync();
    }

    /// <summary>
    /// Retorna o primeiro registro da tabela
    /// </summary>
    /// <param name="expression">Expressão Lambda/param>
    /// <returns>Primeira entidade ou nulo</returns>
    public async Task<TEntity?> GetFirstAsync(Expression<Func<TEntity, bool>> expression)
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        return await Context.Set<TEntity>().FirstOrDefaultAsync(expression ?? (x => true));
    }

    /// <summary>
    /// Retorna o último registro da tabela
    /// </summary>
    /// <returns>Última entidade ou nulo</returns>
    public async Task<TEntity?> GetLastAsync()
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        return await Context.Set<TEntity>().LastOrDefaultAsync();
    }

    /// <summary>
    /// Retorna o último registro da tabela
    /// </summary>
    /// <param name="expression">Expressão Lambda/param>
    /// <returns>Última entidade ou nulo</returns>
    public async Task<TEntity?> GetLastAsync(Expression<Func<TEntity, bool>> expression)
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        return await Context.Set<TEntity>().LastOrDefaultAsync(expression ?? (x => true));
    }

    /// <summary>
    /// Realizar busca de dados na tabela e retornar a quantidade desejada, pulado um determinado valor inicial
    /// </summary>
    /// <param name="expression">Expressão Lambda/param>
    /// <param name="skip">Valor ignorado</param>
    /// <param name="amount">Quantidade desejada</param>
    /// <returns>Lista de entidades como IEnumerable</returns>
    public async Task<IEnumerable<TEntity>> GetSomeAsync(Expression<Func<TEntity, bool>> expression, int skip, int amount)
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        return await Context.Set<TEntity>().Where(expression ?? (x => true)).Skip(skip).Take(amount).ToListAsync();
    }

    /// <summary>
    /// Realiza pesquisa de dados na tabela e pula um determinado valor inicial
    /// </summary>
    /// <param name="expression">Expressão Lambda/param>
    /// <param name="skip">Valor ignorado</param>
    /// <returns>Lista de entidades como IEnumerable</returns>
    public async Task<IEnumerable<TEntity>> SkipAsync(Expression<Func<TEntity, bool>> expression, int skip)
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        return await Context.Set<TEntity>().Where(expression ?? (x => true)).Skip(skip).ToListAsync();
    }

    /// <summary>
    /// Realizar busca de dados na tabela e retornar a quantidade desejada
    /// </summary>
    /// <param name="expression">Expressão Lambda/param>
    /// <param name="amount">Quantidade desejada</param>
    /// <returns>Lista de entidades como IEnumerable</returns>
    public async Task<IEnumerable<TEntity>> TakeAsync(Expression<Func<TEntity, bool>> expression, int amount)
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        return await Context.Set<TEntity>().Where(expression ?? (x => true)).Take(amount).ToListAsync();
    }

    /// <summary>
    /// Conta a quantidade de registro na tabela
    /// </summary>
    /// <returns>Total</returns>
    public async Task<int> CountAsync()
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        return await Context.Set<TEntity>().CountAsync();
    }

    /// <summary>
    /// Conta a quantidade de registro na tabela
    /// </summary>
    /// <param name="expression">Expressão Lambda/param>
    /// <returns>Total</returns>
    public async Task<int> CountAsync(Expression<Func<TEntity, bool>> expression)
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        return await Context.Set<TEntity>().CountAsync(expression ?? (x => true));
    }

    /// <summary>
    /// Verifica se o registro existe <br/>
    /// <i>Nota: se a expressão retornar mais de um resultado, será considerada existente</i>
    /// </summary>
    /// <param name="expression">expressão Lambda</param>
    /// <returns>Se existir, retorna true</returns>
    public async Task<bool> ExistAsync(Expression<Func<TEntity, bool>> expression)
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        TEntity? entity = await Context.Set<TEntity>().FirstOrDefaultAsync(expression ?? (x => true));
        if (entity != null)
            return true;
        else
            return false;
    }

    /// <summary>
    /// Verifica se o registro existe
    /// </summary>
    /// <param name="id">Id do registro</param>
    /// <returns>Se existir, retorna true</returns>
    public async Task<bool> ExistAsync(params object[] id)
    {
        if (Context == null)
            throw new Exception("Contexto não definido ou incorreto.");

        TEntity? entity = await Context.Set<TEntity>().FindAsync(id);
        if (entity != null)
            return true;
        else
            return false;
    }

    #endregion
}

