namespace DocConversion.ConApp.Models
{
    /// Represents a menu item.
    /// Gets or sets the unique key of the menu item.
    /// Gets or sets the displayed text of the menu item.
    /// Gets or sets the action to be performed when the menu item is selected.
    public partial class MenuItem
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key value.
        /// </value>
        public required string Key { get; set; }
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public required string Text { get; set; }
        /// <summary>
        /// Gets or sets the action associated with the property.
        /// </summary>
        /// <value>
        /// The action associated with the property.
        /// </value>
        public required Action Action { get; set; }
    }
}
