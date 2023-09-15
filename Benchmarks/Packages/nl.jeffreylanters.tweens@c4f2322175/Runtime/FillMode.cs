namespace Tweens {
  public enum FillMode {
    /// <summary>
    /// The animation will not be applied before the Tween has started, and will return to its original state after the Tween has ended.
    /// </summary>
    None,
    /// <summary>
    /// The animation will be applied before the Tween has started, but will return to its original state after the Tween has ended.
    /// </summary>
    Forwards,
    /// <summary>
    /// The animation will not be applied before the Tween has started, but will remain in its final state after the Tween has ended.
    /// </summary>
    Backwards,
    /// <summary>
    /// The animation will be applied before the Tween has started, and will remain in its final state after the Tween has ended.
    /// </summary>
    Both,
  }
}