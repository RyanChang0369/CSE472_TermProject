/// <summary>
/// Represents the cached value of an expensive operation. In the case that the
/// expression needs to be re-evaluated, <see cref="SetDirty"/> can be used.
///
/// <br/>
///
/// Authors: Ryan Chang (2024)
/// </summary>
public class CachedExpression<T>
{
    #region Delegate
    /// <summary>
    /// The delegate that is responsible for evaluating the value of the cache.
    /// </summary>
    /// <returns>The evaluated value returned by the expression.</returns>
    public delegate T Expression();
    #endregion

    #region Variables
    /// <summary>
    /// Cached value of the expression.
    /// </summary>
    private T cache;

    /// <inheritdoc cref="Expression"/>
    private readonly Expression expression;
    #endregion

    #region Properties
    /// <summary>
    /// Whether or not the cache is dirty. If the cache is dirty, calling Value
    /// will cause <see cref="expression"/> to be evaluated and the <see
    /// cref="cache"/> updated. You can set the cache to be dirty with <see
    /// cref="SetDirty()"/>.
    /// </summary>
    public bool Dirty { get; private set; } = true;

    /// <summary>
    /// The value of the expression. Will be re-evaluated if <see cref="Dirty"/>
    /// is true, otherwise uses a cached value.
    /// </summary>
    public T Value
    {
        get
        {
            if (Dirty)
            {
                Dirty = false;
                cache = expression();
            }

            return cache;
        }
    }
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a new cached expression.
    /// </summary>
    /// <param name="expression">The delegate that is responsible for evaluating
    /// the value of the cache.</param>
    public CachedExpression(Expression expression)
    {
        Dirty = true;
        this.expression = expression;
    }
    #endregion

    #region Methods
    /// <summary>
    /// Sets the dirty bit.
    /// </summary>
    public void SetDirty()
    {
        Dirty = true;
    }
    #endregion
}