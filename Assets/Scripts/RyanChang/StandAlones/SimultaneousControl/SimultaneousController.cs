using System.Collections;
using NaughtyAttributes;
using UnityEngine;

/// <summary>
/// Allows for the simultaneous (but not concurrent) execution of multiple <see
/// cref="SimultaneousControl"/>.
/// <br/>
/// Authors: Ryan Chang (2023)
/// </summary>
public abstract class SimultaneousController : MonoBehaviour
{
    #region Variables
    [Tooltip("If set to true, look through all applicable children and add " +
        "the simultaneous control to them.")]
    [SerializeField]
    protected bool autoAssign;

    [Tooltip("Includes disabled children.")]
    [ShowIf(nameof(autoAssign))]
    [SerializeField]
    protected bool includeInactive;

    [Tooltip("If true, then start the controller on start.")]
    [SerializeField]
    protected bool autorun;

    [Tooltip("If true, then reset the controller when this controller is " +
        "disabled.")]
    [ShowIf(nameof(autorun))]
    [SerializeField]
    protected bool autoReset;


    [Tooltip("The delay, in seconds, between starting one " +
        "SimultaneousControl and starting the next.")]
    [SerializeField]
    private float controlSwitchDelay = 0.2f;

    [Tooltip("How many controls will be actioning at once?")]
    [SerializeField]
    protected int maxSimultaneous = 3;

    [Tooltip("Number of running controls.")]
    [ReadOnly]
    public int currentSimultaneous = 0;

    [HorizontalLine]
    [Tooltip("If set to true, enables manual editing of the controls field.")]
    [SerializeField]
    private bool manualControl;

    [Tooltip("The controls belonging to this controller.")]
    [InfoBox("Unity doesn't play nicely with fields of abstract classes " +
        "(which Controls is). Use at your own risk.", EInfoBoxType.Warning)]
    [EnableIf(nameof(manualControl))]
    [SerializeField]
    protected SimultaneousControl[] controls;
    #endregion

    #region Properties
    public bool HasBeenInitialized { get; private set; } = false;
    #endregion

    #region Public Methods
    /// <summary>
    /// Starts the controller, allowing it to do whatever it needs to do.
    /// </summary>
    public void StartController()
    {
        InitControlsList();
        ResetControls();
        StartCoroutine(RunController());
        HasBeenInitialized = true;
    }

    /// <summary>
    /// Resets all controls.
    /// </summary>
    public void ResetControls()
    {
        foreach (var control in controls)
        {
            control.Controller = this;
            control.ResetControl();
        }
    }
    #endregion

    #region Internal Methods
    #region Instantiation Helpers
    /// <summary>
    /// Instantiates the control list. This will only be called once in play
    /// mode.
    /// </summary>
    protected void InitControlsList()
    {
        // Do nothing if manual control is enabled.
        if (!manualControl)
            controls = CreateControlsList();

        foreach (var control in controls)
        {
            control.Controller = this;
            control.Instantiate();
        }
    }
    #endregion

    #region MonoBehavior Functions
    private bool started = false;

    protected void OnEnable()
    {
        if (started && autorun && autoReset)
        {
            // Start the controls again.
            StartController();
        }
    }

    protected void Start()
    {
        if (autorun)
        {
            StartController();
        }

        started = true;
    }

    protected void OnDisable()
    {
        foreach (var control in controls)
        {
            control.DisableControl();
        }
    }
    #endregion

    #region Create Control List
    /// <summary>
    /// Creates the controls list.
    /// </summary>
    protected abstract SimultaneousControl[] CreateControlsList();

    /// <summary>
    /// Returns a list of SimultaneousControls, which are present on the
    /// children of this gameObject. If <see cref="autoAssign"/> is true, then
    /// automatically adds the specified type of <see
    /// cref="SimultaneousControl"/> to the child. This version gets the
    /// children automatically based on their type T. See <see
    /// cref="GetControls{S}(Component[])"/> if you want to manually supply the
    /// children.
    /// </summary>
    /// <typeparam name="S">The type of <see cref="SimultaneousControl"/> to
    /// add.</typeparam>
    /// <typeparam name="T">The type of component children we are
    /// targeting.</typeparam>
    /// <returns></returns>
    protected S[] GetControls<S, T>()
        where S : SimultaneousControl where T : Component
    {
        return GetControls<S>(GetComponentsInChildren<T>(includeInactive));
    }

    /// <summary>
    /// Returns a list of SimultaneousControls, which are present on the
    /// children of this gameObject. If <see cref="autoAssign"/> is true, then
    /// automatically adds the specified type of <see
    /// cref="SimultaneousControl"/> to the child. The version requires that the
    /// children be supplied manually. See <see cref="GetControls{S, T}"/> if
    /// you want to get the children automatically based on their type.
    /// </summary>
    /// <typeparam name="S">The type of <see cref="SimultaneousControl"/> to
    /// add.</typeparam>
    /// <param name="childs">The children to add the <see
    /// cref="SimultaneousControl"/> to.</param>
    /// <returns></returns>
    protected S[] GetControls<S>(params Component[] childs)
        where S : SimultaneousControl
    {
        if (autoAssign)
        {
            S[] output = new S[childs.Length];

            for (int i = 0; i < childs.Length; i++)
            {
                output[i] = childs[i].gameObject
                    .AddComponentIfMissing<S>();
            }

            return output;
        }
        else
        {
            return GetComponentsInChildren<S>(includeInactive);
        }
    }
    #endregion

    #region Coroutine
    /// <summary>
    /// Controller main coroutine.
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator RunController()
    {
        // The left index.
        int li = 0;

        // The right index.
        int ri = 0;

        int max = Mathf.Min(maxSimultaneous, controls.Length);

        while (ri < controls.Length)
        {
            while (ri - li < max)
            {
                // Grows ri until we have the required number of typewriters
                // running.
                StartCoroutine(RunControl(controls[ri]));
                ri++;

                // Wait until we have less than number of
                // "simultaneousParagraphs" typewriters running. Coroutines are
                // not multithreaded, so no concurrency concerns.
                yield return new WaitUntil(() =>
                    currentSimultaneous < maxSimultaneous);

                yield return new WaitForSecondsRealtime(controlSwitchDelay);
            }

            // All required typewriters running. Move onto next.
            li++;
        }
    }

    protected virtual IEnumerator RunControl(SimultaneousControl control)
    {
        currentSimultaneous++;

        yield return control.DoAction();

        currentSimultaneous--;
    }
    #endregion
    #endregion
}