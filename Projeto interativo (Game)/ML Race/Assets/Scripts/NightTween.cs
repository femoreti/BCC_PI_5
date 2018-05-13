using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Main tween class.
/// </summary>
public static class NightTween
{

    /// <summary>
    /// Container for all created NightTweenInstances.
    /// </summary>
    private static GameObject container;


    /// <summary>
    /// Used to create a tween object.
    /// </summary>
    /// <param name="targetObject">GameObject to be targeted.</param>
    /// <param name="duration">The duration of a single run of the tween.</param>
    /// <param name="property">Target property to animate.</param>
    /// <param name="finalValue">Final value.</param>
    /// <param name="easeType">Ease type.</param>
    /// <param name="config">Tween config.</param>
    public static NT.Instance Create(GameObject targetObject, float duration, NT.Property property, object finalValue)
    {
        return Create(targetObject, duration, NT.NewConfig.Property(property, finalValue));
    }

    /// <summary>
    /// Used to create a tween object.
    /// </summary>
    /// <param name="targetObject">GameObject to be targeted.</param>
    /// <param name="duration">The duration of a single run of the tween.</param>
    /// <param name="property">Target property to animate.</param>
    /// <param name="finalValue">Final value.</param>
    /// <param name="easeType">Ease type.</param>
    /// <param name="config">Tween config.</param>
    public static NT.Instance Create(GameObject targetObject, float duration, NT.Property property, object finalValue, NT.EaseType easeType)
    {
        return Create(targetObject, duration, NT.NewConfig.Property(property, finalValue).Ease(easeType));
    }

    /// <summary>
    /// Used to create a tween object.
    /// </summary>
    /// <param name="targetObject">GameObject to be targeted.</param>
    /// <param name="duration">The duration of a single run of the tween.</param>
    /// <param name="property">Target property to animate.</param>
    /// <param name="finalValue">Final value.</param>
    /// <param name="easeType">Ease type.</param>
    /// <param name="config">Tween config.</param>
    public static NT.Instance Create(GameObject targetObject, float duration, NT.EaseType easeType)
    {
        return Create(targetObject, duration, NT.NewConfig.Ease(easeType));
    }

    /// <summary>
    /// Used to create a tween object.
    /// </summary>
    /// <param name="targetObject">GameObject to be targeted.</param>
    /// <param name="duration">The duration of a single run of the tween.</param>
    /// <param name="property">Target property to animate.</param>
    /// <param name="finalValue">Final value.</param>
    /// <param name="easeType">Ease type.</param>
    /// <param name="config">Tween config.</param>
    public static NT.Instance Create(GameObject targetObject, float duration, NT.Property property, object finalValue, NT.Config config)
    {
        return Create(targetObject, duration, config.Property(property, finalValue));
    }

    /// <summary>
    /// Used to create a tween object.
    /// </summary>
    /// <param name="targetObject">GameObject to be targeted.</param>
    /// <param name="duration">The duration of a single run of the tween.</param>
    /// <param name="property">Target property to animate.</param>
    /// <param name="finalValue">Final value.</param>
    /// <param name="easeType">Ease type.</param>
    /// <param name="config">Tween config.</param>
    public static NT.Instance Create(GameObject targetObject, float duration, NT.Property property, object finalValue, NT.EaseType easeType, NT.Config config)
    {
        return Create(targetObject, duration, config.Property(property, finalValue).Ease(easeType));
    }

    /// <summary>
    /// Used to create a tween object.
    /// </summary>
    /// <param name="targetObject">GameObject to be targeted.</param>
    /// <param name="duration">The duration of a single run of the tween.</param>
    /// <param name="property">Target property to animate.</param>
    /// <param name="finalValue">Final value.</param>
    /// <param name="easeType">Ease type.</param>
    /// <param name="config">Tween config.</param>
    public static NT.Instance Create(GameObject targetObject, float duration, NT.EaseType easeType, NT.Config config)
    {
        return Create(targetObject, duration, config.Ease(easeType));
    }

    /// <summary>
    /// Used to create a tween object.
    /// </summary>
    /// <param name="targetObject">GameObject to be targeted.</param>
    /// <param name="duration">The duration of a single run of the tween.</param>
    /// <param name="config">Tween config.</param>
    public static NT.Instance Create(GameObject targetObject, float duration, NT.Config config)
    {
        if (targetObject == null) return null;
        if (config == null) return null;
        if (container == null)
        {
            container = new GameObject("NightTween");
            Object.DontDestroyOnLoad(container);
        }

        NT.Instance t = container.AddComponent<NT.Instance>();
        t.Setup(targetObject, duration, config);

        return t;
    }

    /// <summary>
    /// Will find and destroy any objects matching given p_id.
    /// </summary>
    /// <param name="p_id">Object ID.</param>
    /// <param name="setFinalValue">Whether to update the final value or leave it as is.</param>
    /// <param name="preventCallbacks">Prevents all callback invocations.</param>
    public static void Destroy(string p_id, bool setFinalValue = false, bool preventCallbacks = false)
    {
        if (container != null)
        {
            string toTest = "C" + p_id;

            NT.Instance[] instances = container.GetComponents<NT.Instance>();
            for (int x = 0, count = instances.Length; x < count; x++)
            {
                if (instances[x].id == toTest)
                {
                    instances[x].End();
                }
            }
        }
    }

    /// <summary>
    /// Will find and destroy any objects matching given p_intId.
    /// </summary>
    /// <param name="p_intId">Object ID.</param>
    /// <param name="setFinalValue">Whether to update the final value or leave it as is.</param>
    /// <param name="preventCallbacks">Prevents all callback invocations.</param>
    public static void Destroy(int p_intId, bool setFinalValue = false, bool preventCallbacks = false)
    {
        Destroy(p_intId.ToString(), setFinalValue, preventCallbacks);
    }
}

public static class NT
{
    /// <summary>
    /// Same as using the constructor.
    /// </summary>
    public static Config NewConfig
    {
        get
        {
            return new Config();
        }
    }

    /// <summary>
    /// Instance used to control a single object.
    /// </summary>
    public class Instance : MonoBehaviour
    {
        /// <summary>
        /// GameObject being animated.
        /// </summary>
        [SerializeField]
        private GameObject _targetObject;
        /// <summary>
        /// Duration of a single loop.
        /// </summary>
        [SerializeField]
        private float _duration;
        /// <summary>
        /// Number of loops completed.
        /// </summary>
        [SerializeField]
        private int _loopsComplete = 0;
        /// <summary>
        /// Parameters acquired during creation.
        /// </summary>
        [SerializeField]
        private Config.Stored _parameters;
        /// <summary>
        /// Time born.
        /// </summary>
        private float _born = 0f;
        /// <summary>
        /// Time started.
        /// </summary>
        private float _started = 0f;

        /// <summary>
        /// Use regular Update.
        /// </summary>
        private bool useUpdate = false;
        /// <summary>
        /// Use LateUpdate.
        /// </summary>
        private bool useLateUpdate = false;
        /// <summary>
        /// Use FixedUpdate.
        /// </summary>
        private bool useFixedUpdate = false;
        /// <summary>
        /// Use regular Update with real world time.
        /// </summary>
        private bool useTimescaleInd = false;

        /// <summary>
        /// Stores if the delay was completed.
        /// </summary>
        private bool pastDelay = false;

        /// <summary>
        /// Starting values to lerp.
        /// </summary>
        private object[] startingValue;
        /// <summary>
        /// End values to lerp.
        /// </summary>
        private object[] endValue;

        public string id
        {
            get
            {
                return _parameters.id;
            }
        }

        /// <summary>
        /// Setups the NightTweenInstance.
        /// </summary>
        /// <param name="targetObject">GameObject being animated.</param>
        /// <param name="duration">Duration of a single loop.</param>
        /// <param name="config">Parameters acquired during creation.</param>
        public void Setup(GameObject targetObject, float duration, Config config)
        {
            _targetObject = targetObject;
            _duration = duration;
            _parameters = config.stored;
            for (int x = 0; x < _parameters.property.Length; x++)
                if (_parameters.property[x] == Property.copyTransformValue)
                    _parameters.propertyValue[x] = ((Transform)_parameters.propertyValue[x]).position;

            if (_parameters.updateMode == UpdateType.Update) useUpdate = true;
            else if (_parameters.updateMode == UpdateType.LateUpdate) useLateUpdate = true;
            else if (_parameters.updateMode == UpdateType.FixedUpdate) useFixedUpdate = true;
            else if (_parameters.updateMode == UpdateType.TimeScaleIndependentUpdate) useTimescaleInd = true;

            _born = TimeNow();
            _started = _born + _parameters.delay;

            endValue = _parameters.propertyValue;
            startingValue = new object[_parameters.property.Length];
            for (int x = 0; x < startingValue.Length; x++)
            {
                startingValue[x] = GetValue(_parameters.property[x]);
            }

            if (_parameters.startCallback != null)
            {
                _parameters.startCallback.Invoke();
            }
        }

        /// <summary>
        /// Returns current time.
        /// </summary>
        private float TimeNow()
        {
            if (useTimescaleInd) return Time.realtimeSinceStartup;
            else return Time.time;
        }

        /// <summary>
        /// Update happens every frame.
        /// </summary>
        private void Update()
        {
            if (useUpdate || useTimescaleInd)
            {
                ProcessFrame();
            }
        }

        /// <summary>
        /// LateUpdate happens every frame after Update.
        /// </summary>
        private void LateUpdate()
        {
            if (useLateUpdate)
            {
                ProcessFrame();
            }
        }

        /// <summary>
        /// FixelUpdate happens every physics frame.
        /// </summary>
        private void FixedUpdate()
        {
            if (useFixedUpdate)
            {
                ProcessFrame();
            }
        }

        /// <summary>
        /// Bye Bye.
        /// </summary>
        /// <param name="setFinalValue">Whether to update the final value or leave it as is.</param>
        /// <param name="preventCallbacks">Prevents all callback invocations.</param>
        public void End(bool setFinalValue = false, bool preventCallbacks = false)
        {
            if (setFinalValue)
            {
                int length = _parameters.property.Length;
                for (int x = 0; x < length; x++)
                {
                    SetValue(x, Ease(_parameters.easeType, 1));
                }
            }

            if (!preventCallbacks)
            {
                if (_parameters.finishCallback != null)
                {
                    _parameters.finishCallback.Invoke();
                }
            }

            Destroy(this);
        }

        /// <summary>
        /// Processes the current frame.
        /// </summary>
        private void ProcessFrame()
        {
            if (_targetObject == null)
            {
                Destroy(this);
                return;
            }

            if (!pastDelay)
            {
                float delayDelta = TimeNow() - _born;
                if (delayDelta >= _parameters.delay)
                {
                    pastDelay = true;
                }
                return;
            }

            float delta = TimeNow() - _started;
            float lerp = delta;
            if (_duration <= 0)
            {
                lerp = 1f;
            }
            else
            {
                lerp = lerp / _duration;
                lerp = lerp > 1f ? 1f : lerp;
            }

            int length = _parameters.property.Length;
            for (int x = 0; x < length; x++)
            {
                SetValue(x, Ease(_parameters.easeType, lerp));
            }

            if (lerp >= 1f)
            {
                _loopsComplete++;

                if (_parameters.loops < _loopsComplete)
                {
                    if (_parameters.finishCallback != null)
                    {
                        _parameters.finishCallback.Invoke();
                    }

                    Destroy(this);
                    return;
                }

                _started += _duration;

                if (_parameters.loopType == LoopType.Yoyo)
                {
                    object[] temp = startingValue;
                    startingValue = endValue;
                    endValue = temp;
                }

                _parameters.easeType = (EaseType)((int)_parameters.easeType + 1);

                if (_parameters.endCycleCallback != null)
                {
                    _parameters.endCycleCallback.Invoke();
                }
            }

            if (_parameters.updateCallback != null)
            {
                _parameters.updateCallback.Invoke();
            }
        }

        /// <summary>
        /// Gets a value according to NTPropType.
        /// </summary>
        private object GetValue(Property pType)
        {
            if (pType == Property.transformPosition)
                return _targetObject.transform.position;
            else if (pType == Property.transformLocalPosition)
                return _targetObject.transform.localPosition;
            else if (pType == Property.transformRotation)
                return _targetObject.transform.rotation;
            else if (pType == Property.transformLocalScale)
                return _targetObject.transform.localScale;
            else if (pType == Property.transformEulerAngles)
                return _targetObject.transform.eulerAngles;
            else if (pType == Property.spriteRendererColor)
                return _targetObject.GetComponent<SpriteRenderer>().color;
            else if (pType == Property.uiTextColor)
                return _targetObject.GetComponent<Text>().color;
            else if (pType == Property.rectTransformAnchoredPosition)
                return _targetObject.GetComponent<RectTransform>().anchoredPosition;
            else if (pType == Property.rectTransformSizeDelta)
                return _targetObject.GetComponent<RectTransform>().sizeDelta;
            else if (pType == Property.canvasGroupAlpha)
                return _targetObject.GetComponent<CanvasGroup>().alpha;
            else if (pType == Property.followTransform)
                return _targetObject.transform.position;
            else if (pType == Property.copyTransformValue)
                return _targetObject.transform.position;

            return null;
        }

        /// <summary>
        /// Sets a value according to Property type.
        /// </summary>
        private void SetValue(int index, float mult)
        {
            if (_parameters.property[index] == Property.transformPosition)
            {
                Vector3 start = (Vector3)startingValue[index];
                Vector3 end = (Vector3)endValue[index];
                _targetObject.transform.position = start + (end - start) * mult;
            }
            else if (_parameters.property[index] == Property.transformLocalPosition)
            {
                Vector3 start = (Vector3)startingValue[index];
                Vector3 end = (Vector3)endValue[index];
                _targetObject.transform.localPosition = start + (end - start) * mult;
            }
            else if (_parameters.property[index] == Property.transformRotation)
            {
                Quaternion start = (Quaternion)startingValue[index];
                Quaternion end = (Quaternion)endValue[index];
                _targetObject.transform.rotation = Quaternion.Lerp(start, end, mult);
            }
            else if (_parameters.property[index] == Property.transformLocalRotation)
            {
                Quaternion start = (Quaternion)startingValue[index];
                Quaternion end = (Quaternion)endValue[index];
                _targetObject.transform.localRotation = Quaternion.Lerp(start, end, mult);
            }
            else if (_parameters.property[index] == Property.transformLocalScale)
            {
                Vector3 start = (Vector3)startingValue[index];
                Vector3 end = (Vector3)endValue[index];
                _targetObject.transform.localScale = start + (end - start) * mult;
            }
            else if (_parameters.property[index] == Property.transformEulerAngles)
            {
                Vector3 start = (Vector3)startingValue[index];
                Vector3 end = (Vector3)endValue[index];
                _targetObject.transform.rotation = Quaternion.Lerp(Quaternion.Euler(start), Quaternion.Euler(end), mult);
            }
            else if (_parameters.property[index] == Property.transformLocalEulerAngles)
            {
                Vector3 start = (Vector3)startingValue[index];
                Vector3 end = (Vector3)endValue[index];
                _targetObject.transform.localRotation = Quaternion.Lerp(Quaternion.Euler(start), Quaternion.Euler(end), mult);
            }
            else if (_parameters.property[index] == Property.spriteRendererColor)
            {
                Color start = (Color)startingValue[index];
                Color end = (Color)endValue[index];
                _targetObject.GetComponent<SpriteRenderer>().color = Color.Lerp(start, end, mult);
            }
            else if (_parameters.property[index] == Property.uiTextColor)
            {
                Color start = (Color)startingValue[index];
                Color end = (Color)endValue[index];
                _targetObject.GetComponent<Text>().color = Color.Lerp(start, end, mult);
            }
            else if (_parameters.property[index] == Property.rectTransformAnchoredPosition)
            {
                Vector2 start = (Vector2)startingValue[index];
                Vector2 end = (Vector2)endValue[index];
                _targetObject.GetComponent<RectTransform>().anchoredPosition = start + (end - start) * mult;
            }
            else if (_parameters.property[index] == Property.rectTransformSizeDelta)
            {
                Vector2 start = (Vector2)startingValue[index];
                Vector2 end = (Vector2)endValue[index];
                _targetObject.GetComponent<RectTransform>().sizeDelta = start + (end - start) * mult;
            }
            else if (_parameters.property[index] == Property.canvasGroupAlpha)
            {
                float start = (float)startingValue[index];
                float end = (float)endValue[index];
                _targetObject.GetComponent<CanvasGroup>().alpha = start + (end - start) * mult;
            }
            else if (_parameters.property[index] == Property.followTransform)
            {
                Vector3 start = (Vector3)startingValue[index];
                if (endValue[index] != null)
                {
                    Vector3 end = ((Transform)endValue[index]).position;
                    _targetObject.transform.position = start + (end - start) * mult;
                }
            }
            else if (_parameters.property[index] == Property.copyTransformValue)
            {
                Vector3 start = (Vector3)startingValue[index];
                Vector3 end = (Vector3)endValue[index];
                _targetObject.transform.position = start + (end - start) * mult;
            }
        }
    }


    /// <summary>
    /// Property type to be used.
    /// </summary>
    public enum Property
    {
        /// <summary>
        /// Transform.position.
        /// </summary>
        transformPosition,
        /// <summary>
        /// Transform.localPosition.
        /// </summary>
        transformLocalPosition,
        /// <summary>
        /// Transform.rotation.
        /// </summary>
        transformRotation,
        /// <summary>
        /// Transform.localRotation.
        /// </summary>
        transformLocalRotation,
        /// <summary>
        /// Transform.eulerAngles.
        /// </summary>
        transformEulerAngles,
        /// <summary>
        /// Transform.localEulerAngles.
        /// </summary>
        transformLocalEulerAngles,
        /// <summary>
        /// Transform.localScale.
        /// </summary>
        transformLocalScale,
        /// <summary>
        /// UI.Text.color.
        /// </summary>
        uiTextColor,
        /// <summary>
        /// RectTransform.anchoredPosition.
        /// </summary>
        rectTransformAnchoredPosition,
        /// <summary>
        /// RectTransform.sizeDelta.
        /// </summary>
        rectTransformSizeDelta,
        /// <summary>
        /// canvasGroup.alpha
        /// </summary>
        canvasGroupAlpha,
        /// <summary>
        /// SpriteRenderer.color
        /// </summary>
        spriteRendererColor,
        /// <summary>
        /// Transform.position
        /// </summary>
        followTransform,
        /// <summary>
        /// Transform.position
        /// </summary>
        copyTransformValue
    }



    /// <summary>
    /// Loop type.
    /// </summary>
    public enum LoopType
    {
        /// <summary>
        /// Restarts at the initial state.
        /// </summary>
        Restart = 0,
        /// <summary>
        /// Swaps start and end state after a cycle has been completed.
        /// </summary>
        Yoyo = 1
    }

    /// <summary>
    /// Update type.
    /// </summary>
    public enum UpdateType
    {
        /// <summary>
        /// Use regular Update.
        /// </summary>
        Update = 0,
        /// <summary>
        /// Use LateUpdate.
        /// </summary>
        LateUpdate = 1,
        /// <summary>
        /// Use FixedUpdate.
        /// </summary>
        FixedUpdate = 2,
        /// <summary>
        /// Use regular Update with real world time.
        /// </summary>
        TimeScaleIndependentUpdate = 3,
    }

    /// <summary>
    /// NightTween parameters.
    /// </summary>
    [System.Serializable]
    public class Config
    {
        /// <summary>
        /// Basic constructor.
        /// </summary>
        public Config()
        {
            stored = new Stored(0);
        }

        /// <summary>
        /// Parameters stored.
        /// </summary>
        public Stored stored;
        /// <summary>
        /// Unique ID number counter.
        /// </summary>
        private static int numberCount = 0;

        /// <summary>
        /// Returns a unique ID.
        /// </summary>
        private static string NextNumber()
        {
            numberCount++;
            return numberCount.ToString();
        }

        /// <summary>
        /// Used to add a delay before starting the tween.
        /// </summary>
        /// <param name="p_delay">Delay in seconds.</param>
        public Config Delay(float p_delay)
        {
            stored.delay = p_delay;
            return this;
        }
        /// <summary>
        /// Easing function to use.
        /// </summary>
        /// <param name="p_easeType">NightTween.EaseType.</param>
        public Config Ease(EaseType p_easeType)
        {
            stored.easeType = p_easeType;
            return this;
        }
        /// <summary>
        /// Custom ID for identification.
        /// </summary>
        /// <param name="p_id">Unique name.</param>
        public Config Id(string p_id)
        {
            stored.id = "C" + p_id;
            return this;
        }
        /// <summary>
        /// Custom ID for identification.
        /// </summary>
        /// <param name="p_intId">Unique number.</param>
        public Config Id(int p_intId)
        {
            return Id(p_intId.ToString());
        }
        /// <summary>
        /// Number of loops.
        /// </summary>
        /// <param name="p_loops">Default is 0 (no loop). Negative numbers equals int.MaxValue.</param>
        public Config Loops(int p_loops)
        {
            stored.loops = p_loops < 0 ? int.MaxValue : p_loops;
            return this;
        }
        /// <summary>
        /// Number of loops.
        /// </summary>
        /// <param name="p_loops">Default is 0 (no loop). Negative numbers equals int.MaxValue.</param>
        /// <param name="p_loopType">NightTween.LoopType to be used.</param>
        public Config Loops(int p_loops, LoopType p_loopType)
        {
            stored.loops = p_loops < 0 ? int.MaxValue : p_loops;
            stored.loopType = p_loopType;
            return this;
        }
        /// <summary>
        /// Callback to be called on the end of each cycle.
        /// </summary>
        /// <param name="p_function">Callback method.</param>
        public Config OnEndCycle(UnityAction p_function)
        {
            if (stored.endCycleCallback == null)
                stored.endCycleCallback = new UnityEvent();
            stored.endCycleCallback.AddListener(p_function);
            return this;
        }
        /// <summary>
        /// Callback to be called on start.
        /// </summary>
        /// <param name="p_function">Callback method.</param>
        public Config OnStart(UnityAction p_function)
        {
            if (stored.startCallback == null)
                stored.startCallback = new UnityEvent();
            stored.startCallback.AddListener(p_function);
            return this;
        }
        /// <summary>
        /// Callback to be called on update.
        /// </summary>
        /// <param name="p_function">Callback method.</param>
        public Config OnUpdate(UnityAction p_function)
        {
            if (stored.updateCallback == null)
                stored.updateCallback = new UnityEvent();
            stored.updateCallback.AddListener(p_function);
            return this;
        }
        /// <summary>
        /// Callback to be called on finish.
        /// </summary>
        /// <param name="p_function">Callback method.</param>
        public Config OnFinish(UnityAction p_function)
        {
            if (stored.finishCallback == null)
                stored.finishCallback = new UnityEvent();
            stored.finishCallback.AddListener(p_function);
            return this;
        }
        /// <summary>
        /// Property to be changed. You can use more than one property by calling this again.
        /// </summary>
        /// <param name="p_propName">Property type.</param>
        /// <param name="p_endVal">Property value.</param>
        public Config Property(Property p_propName, object p_endVal)
        {
            if (stored.property == null)
            {
                stored.property = new Property[1];
            }
            else
            {
                Property[] newParams = new Property[stored.property.Length + 1];
                for (int x = 0; x < stored.property.Length; x++)
                {
                    newParams[x] = stored.property[x];
                }
                stored.property = newParams;
            }
            stored.property[stored.property.Length - 1] = p_propName;

            if (stored.propertyValue == null)
                stored.propertyValue = new object[1];
            else
            {
                object[] newValues = new object[stored.propertyValue.Length + 1];
                for (int x = 0; x < stored.propertyValue.Length; x++)
                {
                    newValues[x] = stored.propertyValue[x];
                }
                stored.propertyValue = newValues;
            }
            stored.propertyValue[stored.propertyValue.Length - 1] = p_endVal;

            return this;
        }
        /// <summary>
        /// Update mode used.
        /// </summary>
        /// <param name="p_updateType">NightTween.UpdateType. Default is NightTween.UpdateType.Update.</param>
        public Config UpdateMode(UpdateType p_updateType)
        {
            stored.updateMode = p_updateType;
            return this;
        }

        /// <summary>
        /// Stored parameters.
        /// </summary>
        [System.Serializable]
        public struct Stored
        {
            public float delay;
            public EaseType easeType;
            public string id;
            public int loops;
            public LoopType loopType;
            public UnityEvent endCycleCallback;
            public UnityEvent startCallback;
            public UnityEvent finishCallback;
            public UnityEvent updateCallback;
            public Property[] property;
            public object[] propertyValue;
            public UpdateType updateMode;

            /// <summary>
            /// Create a new StoredParams with default values;
            /// </summary>
            /// <param name="mode">Any value.</param>
            public Stored(int mode)
            {
                delay = 0;
                easeType = EaseType.Linear;
                id = NextNumber();
                loops = 0;
                loopType = LoopType.Restart;
                endCycleCallback = null;
                startCallback = null;
                finishCallback = null;
                updateCallback = null;
                property = null;
                propertyValue = null;
                updateMode = UpdateType.Update;
            }
        }
    }


    #region Ease

    /// <summary>
    /// Easing functions.
    /// </summary>
    public enum EaseType
    {
        Linear,
        ExpoEaseOut,
        ExpoEaseIn,
        ExpoEaseInOut,
        ExpoEaseOutIn,
        CircEaseOut,
        CircEaseIn,
        CircEaseInOut,
        CircEaseOutIn,
        QuadEaseOut,
        QuadEaseIn,
        QuadEaseInOut,
        QuadEaseOutIn,
        SineEaseOut,
        SineEaseIn,
        SineEaseInOut,
        SineEaseOutIn,
        CubicEaseOut,
        CubicEaseIn,
        CubicEaseInOut,
        CubicEaseOutIn,
        QuartEaseOut,
        QuartEaseIn,
        QuartEaseInOut,
        QuartEaseOutIn,
        QuintEaseOut,
        QuintEaseIn,
        QuintEaseInOut,
        QuintEaseOutIn,
        ElasticEaseOut,
        ElasticEaseIn,
        ElasticEaseInOut,
        ElasticEaseOutIn,
        BounceEaseOut,
        BounceEaseIn,
        BounceEaseInOut,
        BounceEaseOutIn,
        BackEaseOut,
        BackEaseIn,
        BackEaseInOut,
        BackEaseOutIn
    }

    /// <summary>
    /// Apply Ease function.
    /// </summary>
    /// <param name="type">Ease Type.</param>
    /// <param name="lerp">Value to lerp from 0 to 1.</param>
    private static float Ease(EaseType type, float lerp)
    {
        switch (type)
        {
            case EaseType.Linear:
                return Linear(lerp);
            case EaseType.ExpoEaseOut:
                return ExpoEaseOut(lerp);
            case EaseType.ExpoEaseIn:
                return ExpoEaseIn(lerp);
            case EaseType.ExpoEaseInOut:
                return ExpoEaseInOut(lerp);
            case EaseType.ExpoEaseOutIn:
                return ExpoEaseOutIn(lerp);
            case EaseType.CircEaseOut:
                return CircEaseOut(lerp);
            case EaseType.CircEaseIn:
                return CircEaseIn(lerp);
            case EaseType.CircEaseInOut:
                return CircEaseInOut(lerp);
            case EaseType.CircEaseOutIn:
                return CircEaseOutIn(lerp);
            case EaseType.QuadEaseOut:
                return QuadEaseOut(lerp);
            case EaseType.QuadEaseIn:
                return QuadEaseIn(lerp);
            case EaseType.QuadEaseInOut:
                return QuadEaseInOut(lerp);
            case EaseType.QuadEaseOutIn:
                return QuadEaseOutIn(lerp);
            case EaseType.SineEaseOut:
                return SineEaseOut(lerp);
            case EaseType.SineEaseIn:
                return SineEaseIn(lerp);
            case EaseType.SineEaseInOut:
                return SineEaseInOut(lerp);
            case EaseType.SineEaseOutIn:
                return SineEaseOutIn(lerp);
            case EaseType.CubicEaseOut:
                return CubicEaseOut(lerp);
            case EaseType.CubicEaseIn:
                return CubicEaseIn(lerp);
            case EaseType.CubicEaseInOut:
                return CubicEaseInOut(lerp);
            case EaseType.CubicEaseOutIn:
                return CubicEaseOutIn(lerp);
            case EaseType.QuartEaseOut:
                return QuartEaseOut(lerp);
            case EaseType.QuartEaseIn:
                return QuartEaseIn(lerp);
            case EaseType.QuartEaseInOut:
                return QuartEaseInOut(lerp);
            case EaseType.QuartEaseOutIn:
                return QuartEaseOutIn(lerp);
            case EaseType.QuintEaseOut:
                return QuintEaseOut(lerp);
            case EaseType.QuintEaseIn:
                return QuintEaseIn(lerp);
            case EaseType.QuintEaseInOut:
                return QuintEaseInOut(lerp);
            case EaseType.QuintEaseOutIn:
                return QuintEaseOutIn(lerp);
            case EaseType.ElasticEaseOut:
                return ElasticEaseOut(lerp);
            case EaseType.ElasticEaseIn:
                return ElasticEaseIn(lerp);
            case EaseType.ElasticEaseInOut:
                return ElasticEaseInOut(lerp);
            case EaseType.ElasticEaseOutIn:
                return ElasticEaseOutIn(lerp);
            case EaseType.BounceEaseOut:
                return BounceEaseOut(lerp);
            case EaseType.BounceEaseIn:
                return BounceEaseIn(lerp);
            case EaseType.BounceEaseInOut:
                return BounceEaseInOut(lerp);
            case EaseType.BounceEaseOutIn:
                return BounceEaseOutIn(lerp);
            case EaseType.BackEaseOut:
                return BackEaseOut(lerp);
            case EaseType.BackEaseIn:
                return BackEaseIn(lerp);
            case EaseType.BackEaseInOut:
                return BackEaseInOut(lerp);
            case EaseType.BackEaseOutIn:
                return BackEaseOutIn(lerp);
            default:
                break;
        }
        return lerp;
    }



    #region Interpolation Equations

    private static float Linear(float t)
    {
        return t;
    }

    private static float ExpoEaseOut(float t)
    {
        return (t == 1f) ? 1f : 1f * (-Mathf.Pow(2f, -10f * t) + 1f);
    }

    private static float ExpoEaseIn(float t)
    {
        return (t == 0f) ? 0f : 1f * Mathf.Pow(2f, 10f * (t - 1f)) + 0f;
    }

    private static float ExpoEaseInOut(float t)
    {
        if (t == 0f)
            return 0f;

        if (t == 1f)
            return 1f;

        if ((t /= 0.5f) < 1f)
            return 0.5f * Mathf.Pow(2f, 10f * (t - 1f));

        return 0.5f * (-Mathf.Pow(2f, -10f * --t) + 2f);
    }

    private static float ExpoEaseOutIn(float t)
    {
        if (t < 0.5f)
            return ExpoEaseOut(t * 2f) * 0.5f;

        return ExpoEaseIn((t - 0.5f) * 2f) * 0.5f + 0.5f;
    }

    private static float CircEaseOut(float t)
    {
        return 1f * Mathf.Sqrt(1f - (t = t - 1f) * t);
    }

    private static float CircEaseIn(float t)
    {
        return -1f * (Mathf.Sqrt(1f - (t /= 1f) * t) - 1f);
    }

    private static float CircEaseInOut(float t)
    {
        if ((t /= 0.5f) < 1f)
            return -0.5f * (Mathf.Sqrt(1f - t * t) - 1f);

        return 0.5f * (Mathf.Sqrt(1f - (t -= 2f) * t) + 1f);
    }

    private static float CircEaseOutIn(float t)
    {
        if (t < 0.5f)
            return CircEaseOut(t * 2f) * 0.5f;

        return CircEaseIn((t - 0.5f) * 2f) * 0.5f + 0.5f;
    }

    private static float QuadEaseOut(float t)
    {
        return -1f * (t /= 1f) * (t - 2f);
    }

    private static float QuadEaseIn(float t)
    {
        return 1f * (t /= 1f) * t;
    }

    private static float QuadEaseInOut(float t)
    {
        if ((t /= 0.5f) < 1f)
            return 0.5f * t * t;

        return -0.5f * ((--t) * (t - 2f) - 1f);
    }

    private static float QuadEaseOutIn(float t)
    {
        if (t < 0.5f)
            return QuadEaseOut(t * 2f) * 0.5f;

        return QuadEaseIn((t - 0.5f) * 2f) * 0.5f + 0.5f;
    }

    private static float SineEaseOut(float t)
    {
        return 1f * Mathf.Sin(t * (Mathf.PI * 0.5f));
    }

    private static float SineEaseIn(float t)
    {
        return -1f * Mathf.Cos(t * (Mathf.PI * 0.5f)) + 1f;
    }

    private static float SineEaseInOut(float t)
    {
        if (t < 0.5f)
            return SineEaseIn(t * 2f) * 0.5f;

        return SineEaseOut((t - 0.5f) * 2f) * 0.5f + 0.5f;
    }

    private static float SineEaseOutIn(float t)
    {
        if (t < 0.5f)
            return SineEaseOut(t * 2f) * 0.5f;

        return SineEaseIn((t - 0.5f) * 2f) * 0.5f + 0.5f;
    }

    private static float CubicEaseOut(float t)
    {
        return 1f * ((t = t - 1f) * t * t + 1f);
    }

    private static float CubicEaseIn(float t)
    {
        return 1f * (t /= 1f) * t * t;
    }

    private static float CubicEaseInOut(float t)
    {
        if ((t /= 0.5f) < 1f)
            return 0.5f * t * t * t;

        return 0.5f * ((t -= 2f) * t * t + 2f);
    }

    private static float CubicEaseOutIn(float t)
    {
        if (t < 0.5f)
            return CubicEaseOut(t * 2f) * 0.5f;

        return CubicEaseIn((t - 0.5f) * 2f) * 0.5f + 0.5f;
    }

    private static float QuartEaseOut(float t)
    {
        return -1f * ((t = t - 1f) * t * t * t - 1f);
    }

    private static float QuartEaseIn(float t)
    {
        return 1f * (t /= 1f) * t * t * t;
    }

    private static float QuartEaseInOut(float t)
    {
        if ((t /= 0.5f) < 1f)
            return 0.5f * t * t * t * t;

        return -0.5f * ((t -= 2f) * t * t * t - 2f);
    }

    private static float QuartEaseOutIn(float t)
    {
        if (t < 0.5f)
            return QuartEaseOut(t * 2f) * 0.5f;

        return QuartEaseIn((t - 0.5f) * 2f) * 0.5f + 0.5f;
    }

    private static float QuintEaseOut(float t)
    {
        return 1f * ((t = t - 1f) * t * t * t * t + 1f);
    }

    private static float QuintEaseIn(float t)
    {
        return 1f * (t /= 1f) * t * t * t * t;
    }

    private static float QuintEaseInOut(float t)
    {
        if ((t /= 0.5f) < 1f)
            return 0.5f * t * t * t * t * t;
        return 0.5f * ((t -= 2f) * t * t * t * t + 2f);
    }

    private static float QuintEaseOutIn(float t)
    {
        if (t < 0.5f)
            return QuintEaseOut(t * 2f) * 0.5f;
        return QuintEaseIn((t - 0.5f) * 2f) * 0.5f + 0.5f;
    }

    private static float ElasticEaseOut(float t)
    {
        if ((t /= 1f) == 1f)
            return 1f;

        float p = 1f * .3f;
        float s = p / 4f;

        return (1f * Mathf.Pow(2f, -10f * t) * Mathf.Sin((t * 1f - s) * (2f * Mathf.PI) / p) + 1f + 0f);
    }

    private static float ElasticEaseIn(float t)
    {
        if ((t /= 1f) == 1f)
            return 1f;

        float p = 1f * .3f;
        float s = p / 4f;

        return -(1f * Mathf.Pow(2f, 10f * (t -= 1f)) * Mathf.Sin((t * 1f - s) * (2f * Mathf.PI) / p));
    }

    private static float ElasticEaseInOut(float t)
    {
        if ((t /= 0.5f) == 2f)
            return 1f;

        float p = 1f * (.3f * 1.5f);
        float s = p / 4f;

        if (t < 1f)
            return -.5f * (1f * Mathf.Pow(2f, 10f * (t -= 1f)) * Mathf.Sin((t * 1f - s) * (2f * Mathf.PI) / p));
        return 1f * Mathf.Pow(2f, -10f * (t -= 1f)) * Mathf.Sin((t * 1f - s) * (2f * Mathf.PI) / p) * .5f + 1f;
    }

    private static float ElasticEaseOutIn(float t)
    {
        if (t < 0.5f)
            return ElasticEaseOut(t * 2f) * 0.5f;
        return ElasticEaseIn((t - 0.5f) * 2f) * 0.5f + 0.5f;
    }

    private static float BounceEaseOut(float t)
    {
        if ((t /= 1f) < (1f / 2.75f))
            return 1f * (7.5625f * t * t);
        else if (t < (2f / 2.75f))
            return 1f * (7.5625f * (t -= (1.5f / 2.75f)) * t + .75f);
        else if (t < (2.5f / 2.75f))
            return 1f * (7.5625f * (t -= (2.25f / 2.75f)) * t + .9375f);
        else
            return 1f * (7.5625f * (t -= (2.625f / 2.75f)) * t + .984375f);
    }

    private static float BounceEaseIn(float t)
    {
        return 1f - BounceEaseOut(1f - t);
    }

    private static float BounceEaseInOut(float t)
    {
        if (t < 0.5f)
            return BounceEaseIn(t * 2f) * 0.5f;
        else
            return ((t - 0.5f) * 2f) * 0.5f + 0.5f;
    }

    private static float BounceEaseOutIn(float t)
    {
        if (t < 0.5f)
            return BounceEaseOut(t * 2f) * 0.5f;
        return ((t - 0.5f) * 2f) * 0.5f + 0.5f;
    }

    private static float BackEaseOut(float t)
    {
        return 1f * ((t = t - 1f) * t * ((1.70158f + 1f) * t + 1.70158f) + 1f);
    }

    private static float BackEaseIn(float t)
    {
        return 1f * (t /= 1f) * t * ((1.70158f + 1f) * t - 1.70158f);
    }

    private static float BackEaseInOut(float t)
    {
        float s = 1.70158f;
        if ((t /= 0.5f) < 1f)
            return 0.5f * (t * t * (((s *= (1.525f)) + 1f) * t - s));
        return 0.5f * ((t -= 2) * t * (((s *= (1.525f)) + 1f) * t + s) + 2f);
    }

    private static float BackEaseOutIn(float t)
    {
        if (t < 0.5f)
            return (t * 2f) * 0.5f;
        return BackEaseIn((t - 0.5f) * 2f) * 0.5f + 0.5f;
    }

    #endregion

    #endregion
}