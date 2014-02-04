using System;
using System.Collections.Generic;
using System.Linq;
using XamlStyler.Core.Options;

namespace XamlStyler.Core.Model
{
    public class AttributeOrderRules
    {
        #region Fields

        private readonly IDictionary<string, AttributeOrderRule> _internalDictionary;

        #endregion Fields

        #region Constructors

        public AttributeOrderRules(IStylerOptions options)
        {
            _internalDictionary = new Dictionary<string, AttributeOrderRule>();

            Populate(options.AttributeOrderWpfNamespace, AttributeTokenTypeEnum.WPF_NAMESPACE)
                .Populate(options.AttributeOrderClass, AttributeTokenTypeEnum.CLASS)
                .Populate(options.AttributeOrderKey, AttributeTokenTypeEnum.KEY)
                .Populate(options.AttributeOrderName, AttributeTokenTypeEnum.NAME)
                .Populate(options.AttributeOrderAttachedLayout, AttributeTokenTypeEnum.ATTACHED_LAYOUT)
                .Populate(options.AttributeOrderCoreLayout, AttributeTokenTypeEnum.CORE_LAYOUT)
                .Populate(options.AttributeOrderAlignmentLayout, AttributeTokenTypeEnum.ALIGNMENT_LAYOUT)
                .Populate(options.AttributeOrderOthers, AttributeTokenTypeEnum.OTHER)
                .Populate(options.AttributeOrderBlendRelated, AttributeTokenTypeEnum.BLEND_RELATED);
        }

        #endregion Constructors

        #region Methods

        public bool ContainsRuleFor(string name)
        {
            return _internalDictionary.Keys.Contains(name);
        }

        public AttributeOrderRule GetRuleFor(string attributeName)
        {
            AttributeOrderRule result;

            if (_internalDictionary.Keys.Contains(attributeName))
            {
                result = _internalDictionary[attributeName];
            }
            else
            {
                AttributeTokenTypeEnum tempAttributeTokenType = attributeName.StartsWith("xmlns") ? 
                    AttributeTokenTypeEnum.OTHER_NAMESPACE : AttributeTokenTypeEnum.OTHER;

                result = new AttributeOrderRule
                             {
                                 AttributeTokenType = tempAttributeTokenType,
                                 Priority = 0
                             };
            }

            return result;
        }

        private AttributeOrderRules Populate(string option, AttributeTokenTypeEnum tokenType)
        {
            if (!String.IsNullOrWhiteSpace(option))
            {
                int priority = 1;

                string[] attributeNames = option.Split(',')
                    .Where(x => !String.IsNullOrWhiteSpace(x))
                    .Select(x => x.Trim())
                    .ToArray();

                foreach (string attributeName in attributeNames)
                {
                    _internalDictionary[attributeName] = new AttributeOrderRule
                                                             {
                                                                 AttributeTokenType = tokenType,
                                                                 Priority = priority
                                                             };

                    priority++;
                }
            }

            return this;
        }

        #endregion Methods
    }
}