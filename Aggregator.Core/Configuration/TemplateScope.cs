﻿using System;
using System.Globalization;
using System.Linq;

using Aggregator.Core.Extensions;
using Aggregator.Core.Interfaces;

namespace Aggregator.Core.Configuration
{
    /// <summary>
    /// Implements a <see cref="PolicyScope"/> that allows the user to bind to a specific Process Template name.
    /// </summary>
    /// <remarks>Version checks are not (yet) possible with the On-premise servers. Don't use them for now.</remarks>
    public class TemplateScope : PolicyScope
    {
        private const string TemplateNameKey = "Process Template";

        /// <summary>
        /// The template name to match on.
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        /// The minimum (inclusive) version match on.
        /// </summary>
        /// <remarks>Version checks are not (yet) possible with the On-premise servers. Don't use them for now.</remarks>
        public string MinVersion { get; set; }

        /// <summary>
        /// The maximum (inclusive) version match on.
        /// </summary>
        /// <remarks>Version checks are not (yet) possible with the On-premise servers. Don't use them for now.</remarks>
        public string MaxVersion { get; set; }

        /// <summary>
        /// The Process template id (guid) to match on.
        /// </summary>
        public string TemplateTypeId { get; set; }

        public override string DisplayName
        {
            get
            {
                return string.Format("ProcessTemplate({0}/{1})", this.TemplateName, this.TemplateTypeId);
            }
        }

        /// <summary>
        /// Checks whether this policy matches the request.
        /// </summary>
        /// <param name="currentRequestContext">The requestcontext of the TFS event</param>
        /// <param name="currentNotification">The notification holding the WorkItemChangedEvent.</param>
        /// <returns>true if the policy matches all supplied checks.</returns>
        public override ScopeMatchResult Matches(IRequestContext currentRequestContext, INotification currentNotification)
        {
            var res = new ScopeMatchResult();

            var info = GetTemplateInfo(currentRequestContext, currentNotification);
            IProcessTemplateVersion currentversion = info.Item1;
            IProjectProperty templateNameProperty = info.Item2;

            string processTemplateDescription = string.Format(
                "{0} v{1}.{2} [{3}]",
                templateNameProperty?.Value,
                currentversion.Major,
                currentversion.Minor,
                currentversion.TypeId);

            res.Add(processTemplateDescription);

            res.Success = this.MatchesName(templateNameProperty)
                   && this.MatchesId(currentversion)
                   && this.MatchesMinVersion(currentversion)
                   && this.MatchesMaxVersion(currentversion);
            return res;
        }

        private static Tuple<IProcessTemplateVersion, IProjectProperty> GetTemplateInfo(IRequestContext currentRequestContext, INotification currentNotification)
        {
            var currentversion = currentRequestContext.GetCurrentProjectProcessVersion(new Uri(currentNotification.ProjectUri));
            IProjectProperty[] properties = currentRequestContext.GetProjectProperties(new Uri(currentNotification.ProjectUri));
            var templateNameProperty = properties.FirstOrDefault(
                    p => string.Equals(TemplateNameKey, p.Name, StringComparison.OrdinalIgnoreCase));
            return Tuple.Create(currentversion, templateNameProperty);
        }

        private bool MatchesMaxVersion(IProcessTemplateVersion currentversion)
        {
            if (string.IsNullOrWhiteSpace(this.MaxVersion))
            {
                return true;
            }

            var current = Version.Parse(
                string.Format(CultureInfo.InvariantCulture, "{0}.{1}", currentversion.Major, currentversion.Minor));
            var max = Version.Parse(this.MaxVersion);

            return current <= max;
        }

        private bool MatchesMinVersion(IProcessTemplateVersion currentversion)
        {
            if (string.IsNullOrWhiteSpace(this.MinVersion))
            {
                return true;
            }

            var current = Version.Parse(
                string.Format(CultureInfo.InvariantCulture, "{0}.{1}", currentversion.Major, currentversion.Minor));
            var min = Version.Parse(this.MinVersion);

            return current >= min;
        }

        private bool MatchesId(IProcessTemplateVersion currentversion)
        {
            if (string.IsNullOrWhiteSpace(this.TemplateTypeId))
            {
                return true;
            }

            return currentversion.TypeId.Equals(new Guid(this.TemplateTypeId));
        }

        private bool MatchesName(IProjectProperty templateNameProperty)
        {
            if (string.IsNullOrWhiteSpace(this.TemplateName))
            {
                return true;
            }

            return this.TemplateName.SameAs(templateNameProperty?.Value);
        }
    }
}