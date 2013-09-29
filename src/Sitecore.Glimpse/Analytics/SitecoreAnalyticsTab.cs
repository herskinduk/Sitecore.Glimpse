﻿using System;

using Glimpse.Core.Extensibility;
using Glimpse.Core.Tab.Assist;
using Sitecore.Glimpse.Infrastructure;

namespace Sitecore.Glimpse.Analytics
{
    public class SitecoreAnalyticsTab : TabBase, IDocumentation
    {
        private readonly ISitecoreRequest _sitecoreRequest;

        public SitecoreAnalyticsTab()
            : this(new SitecoreAnalyticsForRequest())
        {
        }

        public SitecoreAnalyticsTab(ISitecoreRequest sitecoreRequest)
        {
            _sitecoreRequest = sitecoreRequest;
        }

        public override object GetData(ITabContext context)
        {
            try
            {
                var sitecoreData = _sitecoreRequest.GetData();

                if (!sitecoreData.HasData()) return null;

                var analyicsOverviewSection = new AnalyicsOverviewSection(sitecoreData).Create();
                var analticsSummary = new AnalticsSummary(sitecoreData).Create();

                if ((string.IsNullOrEmpty(analticsSummary)) || (analyicsOverviewSection == null)) return null;

                var plugin = Plugin.Create("Visitor", analticsSummary);
                plugin.AddRow().Column("Overview").Column(analyicsOverviewSection).Selected();

                var goalsSection = new GoalsSection(sitecoreData).Create();
                var pageViewsSection = new PageViewsSection(sitecoreData).Create();

                if (goalsSection != null)
                    plugin.AddRow().Column("Goals").Column(goalsSection).Info();

                if (pageViewsSection != null)
                    plugin.AddRow().Column("Page Views").Column(pageViewsSection).Quiet();


                // TODO reinstate the profile and pattern sections to run with generic DMS implementations rather than Officecore specific IDs

//                var patternsSection = PatternsSection.Create(sitecoreData);
//                var profilesSection = ProfilesSection.Create(sitecoreData);
//
//
//                if (patternsSection != null) 
//                    plugin.Section("Pattern", patternsSection);
//
//                if (profilesSection != null) 
//                    plugin.Section("Profiles", profilesSection);

                return plugin;
            }
            catch (Exception ex)
            {
                return new { Exception = ex };
            }
        }

        public override string Name
        {
            get { return "Sitecore Analytics"; }
        }

        public string DocumentationUri { get { return Constants.Wiki.Url; } }
    }
}