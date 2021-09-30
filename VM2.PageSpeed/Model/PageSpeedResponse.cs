using Newtonsoft.Json;
using System.Collections.Generic;

namespace VM2.PageSpeed
{
    public class LighthouseAuditResultV5
    {
        /// <summary>The description of the audit.</summary>
        [JsonProperty("description")]
        public virtual string Description { get; set; }

        /// <summary>Freeform details section of the audit.</summary>
        [JsonProperty("details")]
        public virtual IDictionary<string, object> Details { get; set; }

        /// <summary>The value that should be displayed on the UI for this audit.</summary>
        [JsonProperty("displayValue")]
        public virtual string DisplayValue { get; set; }

        /// <summary>An error message from a thrown error inside the audit.</summary>
        [JsonProperty("errorMessage")]
        public virtual string ErrorMessage { get; set; }

        /// <summary>An explanation of the errors in the audit.</summary>
        [JsonProperty("explanation")]
        public virtual string Explanation { get; set; }

        /// <summary>The audit's id.</summary>
        [JsonProperty("id")]
        public virtual string Id { get; set; }

        [JsonProperty("score")]
        public virtual object Score { get; set; }

        /// <summary>The enumerated score display mode.</summary>
        [JsonProperty("scoreDisplayMode")]
        public virtual string ScoreDisplayMode { get; set; }

        /// <summary>The human readable title.</summary>
        [JsonProperty("title")]
        public virtual string Title { get; set; }

        [JsonProperty("warnings")]
        public virtual object Warnings { get; set; }

        /// <summary>The ETag of the item.</summary>
        public virtual string ETag { get; set; }
    }

    public class LighthouseCategoryV5
    {
        /// <summary>An array of references to all the audit members of this category.</summary>
        [JsonProperty("auditRefs")]
        public virtual List<LighthouseCategoryV5.AuditRefsData> AuditRefs { get; set; }

        /// <summary>A more detailed description of the category and its importance.</summary>
        [JsonProperty("description")]
        public virtual string Description { get; set; }

        /// <summary>The string identifier of the category.</summary>
        [JsonProperty("id")]
        public virtual string Id { get; set; }

        /// <summary>A description for the manual audits in the category.</summary>
        [JsonProperty("manualDescription")]
        public virtual string ManualDescription { get; set; }

        [JsonProperty("score")]
        public virtual object Score { get; set; }

        /// <summary>The human-friendly name of the category.</summary>
        [JsonProperty("title")]
        public virtual string Title { get; set; }

        /// <summary>The ETag of the item.</summary>
        public virtual string ETag { get; set; }


        public class AuditRefsData
        {
            /// <summary>The category group that the audit belongs to (optional).</summary>
            [JsonProperty("group")]
            public virtual string Group { get; set; }

            /// <summary>The audit ref id.</summary>
            [JsonProperty("id")]
            public virtual string Id { get; set; }

            /// <summary>The weight this audit's score has on the overall category score.</summary>
            [JsonProperty("weight")]
            public virtual System.Nullable<double> Weight { get; set; }

        }
    }

    public class LighthouseResultV5
    {
        /// <summary>Map of audits in the LHR.</summary>
        [JsonProperty("audits")]
        public virtual IDictionary<string, LighthouseAuditResultV5> Audits { get; set; }

        /// <summary>Map of categories in the LHR.</summary>
        [JsonProperty("categories")]
        public virtual LighthouseResultV5.CategoriesData Categories { get; set; }

        /// <summary>Map of category groups in the LHR.</summary>
        [JsonProperty("categoryGroups")]
        public virtual IDictionary<string, LighthouseResultV5.CategoryGroupsDataElement> CategoryGroups { get; set; }

        /// <summary>The configuration settings for this LHR.</summary>
        [JsonProperty("configSettings")]
        public virtual LighthouseResultV5.ConfigSettingsData ConfigSettings { get; set; }

        /// <summary>Environment settings that were used when making this LHR.</summary>
        [JsonProperty("environment")]
        public virtual LighthouseResultV5.EnvironmentData Environment { get; set; }

        /// <summary>The time that this run was fetched.</summary>
        [JsonProperty("fetchTime")]
        public virtual string FetchTime { get; set; }

        /// <summary>The final resolved url that was audited.</summary>
        [JsonProperty("finalUrl")]
        public virtual string FinalUrl { get; set; }

        /// <summary>The internationalization strings that are required to render the LHR.</summary>
        [JsonProperty("i18n")]
        public virtual I18nData I18n { get; set; }

        /// <summary>The lighthouse version that was used to generate this LHR.</summary>
        [JsonProperty("lighthouseVersion")]
        public virtual string LighthouseVersion { get; set; }

        /// <summary>The original requested url.</summary>
        [JsonProperty("requestedUrl")]
        public virtual string RequestedUrl { get; set; }

        /// <summary>List of all run warnings in the LHR. Will always output to at least `[]`.</summary>
        [JsonProperty("runWarnings")]
        public virtual IList<object> RunWarnings { get; set; }

        /// <summary>A top-level error message that, if present, indicates a serious enough problem that this Lighthouse
        /// result may need to be discarded.</summary>
        [JsonProperty("runtimeError")]
        public virtual RuntimeErrorData RuntimeError { get; set; }

        /// <summary>The Stack Pack advice strings.</summary>
        [JsonProperty("stackPacks")]
        public virtual IList<StackPacksData> StackPacks { get; set; }

        /// <summary>Timing information for this LHR.</summary>
        [JsonProperty("timing")]
        public virtual TimingData Timing { get; set; }

        /// <summary>The user agent that was used to run this LHR.</summary>
        [JsonProperty("userAgent")]
        public virtual string UserAgent { get; set; }

        /// <summary>The ETag of the item.</summary>
        public virtual string ETag { get; set; }

        /// <summary>Map of categories in the LHR.</summary>
        public class CategoriesData
        {
            public CategoriesData()
            {
                Accessibility = new LighthouseCategoryV5();
                BestPractices = new LighthouseCategoryV5();
                Performance = new LighthouseCategoryV5();
                Pwa = new LighthouseCategoryV5();
                Seo = new LighthouseCategoryV5();
            }

            /// <summary>The accessibility category, containing all accessibility related audits.</summary>
            [JsonProperty("accessibility")]
            public virtual LighthouseCategoryV5 Accessibility { get; set; }

            /// <summary>The best practices category, containing all web best practice related audits.</summary>
            [JsonProperty("best-practices")]
            public virtual LighthouseCategoryV5 BestPractices { get; set; }

            /// <summary>The performance category, containing all performance related audits.</summary>
            [JsonProperty("performance")]
            public virtual LighthouseCategoryV5 Performance { get; set; }

            /// <summary>The Progressive-Web-App (PWA) category, containing all pwa related audits.</summary>
            [JsonProperty("pwa")]
            public virtual LighthouseCategoryV5 Pwa { get; set; }

            /// <summary>The Search-Engine-Optimization (SEO) category, containing all seo related audits.</summary>
            [JsonProperty("seo")]
            public virtual LighthouseCategoryV5 Seo { get; set; }

        }

        /// <summary>A grouping contained in a category that groups similar audits together.</summary>
        public class CategoryGroupsDataElement
        {
            /// <summary>An optional human readable description of the category group.</summary>
            [JsonProperty("description")]
            public virtual string Description { get; set; }

            /// <summary>The title of the category group.</summary>
            [JsonProperty("title")]
            public virtual string Title { get; set; }

        }

        /// <summary>The configuration settings for this LHR.</summary>
        public class ConfigSettingsData
        {
            /// <summary>The form factor the emulation should use.</summary>
            [JsonProperty("emulatedFormFactor")]
            public virtual string EmulatedFormFactor { get; set; }

            /// <summary>The locale setting.</summary>
            [JsonProperty("locale")]
            public virtual string Locale { get; set; }

            [JsonProperty("onlyCategories")]
            public virtual object OnlyCategories { get; set; }

        }

        /// <summary>Environment settings that were used when making this LHR.</summary>
        public class EnvironmentData
        {
            /// <summary>The benchmark index number that indicates rough device class.</summary>
            [JsonProperty("benchmarkIndex")]
            public virtual System.Nullable<double> BenchmarkIndex { get; set; }

            /// <summary>The user agent string of the version of Chrome used.</summary>
            [JsonProperty("hostUserAgent")]
            public virtual string HostUserAgent { get; set; }

            /// <summary>The user agent string that was sent over the network.</summary>
            [JsonProperty("networkUserAgent")]
            public virtual string NetworkUserAgent { get; set; }

        }

        /// <summary>The internationalization strings that are required to render the LHR.</summary>
        public class I18nData
        {
            /// <summary>Internationalized strings that are formatted to the locale in configSettings.</summary>
            [JsonProperty("rendererFormattedStrings")]
            public virtual RendererFormattedStringsData RendererFormattedStrings { get; set; }

            /// <summary>Internationalized strings that are formatted to the locale in configSettings.</summary>
            public class RendererFormattedStringsData
            {
                /// <summary>The tooltip text on an expandable chevron icon.</summary>
                [JsonProperty("auditGroupExpandTooltip")]
                public virtual string AuditGroupExpandTooltip { get; set; }

                /// <summary>The label for the initial request in a critical request chain.</summary>
                [JsonProperty("crcInitialNavigation")]
                public virtual string CrcInitialNavigation { get; set; }

                /// <summary>The label for values shown in the summary of critical request chains.</summary>
                [JsonProperty("crcLongestDurationLabel")]
                public virtual string CrcLongestDurationLabel { get; set; }

                /// <summary>The label shown next to an audit or metric that has had an error.</summary>
                [JsonProperty("errorLabel")]
                public virtual string ErrorLabel { get; set; }

                /// <summary>The error string shown next to an erroring audit.</summary>
                [JsonProperty("errorMissingAuditInfo")]
                public virtual string ErrorMissingAuditInfo { get; set; }

                /// <summary>The title of the lab data performance category.</summary>
                [JsonProperty("labDataTitle")]
                public virtual string LabDataTitle { get; set; }

                /// <summary>The disclaimer shown under performance explaning that the network can vary.</summary>
                [JsonProperty("lsPerformanceCategoryDescription")]
                public virtual string LsPerformanceCategoryDescription { get; set; }

                /// <summary>The heading shown above a list of audits that were not computerd in the run.</summary>
                [JsonProperty("manualAuditsGroupTitle")]
                public virtual string ManualAuditsGroupTitle { get; set; }

                /// <summary>The heading shown above a list of audits that do not apply to a page.</summary>
                [JsonProperty("notApplicableAuditsGroupTitle")]
                public virtual string NotApplicableAuditsGroupTitle { get; set; }

                /// <summary>The heading for the estimated page load savings opportunity of an audit.</summary>
                [JsonProperty("opportunityResourceColumnLabel")]
                public virtual string OpportunityResourceColumnLabel { get; set; }

                /// <summary>The heading for the estimated page load savings of opportunity audits.</summary>
                [JsonProperty("opportunitySavingsColumnLabel")]
                public virtual string OpportunitySavingsColumnLabel { get; set; }

                /// <summary>The heading that is shown above a list of audits that are passing.</summary>
                [JsonProperty("passedAuditsGroupTitle")]
                public virtual string PassedAuditsGroupTitle { get; set; }

                /// <summary>The label that explains the score gauges scale (0-49, 50-89, 90-100).</summary>
                [JsonProperty("scorescaleLabel")]
                public virtual string ScorescaleLabel { get; set; }

                /// <summary>The label shown preceding important warnings that may have invalidated an entire
                /// report.</summary>
                [JsonProperty("toplevelWarningsMessage")]
                public virtual string ToplevelWarningsMessage { get; set; }

                /// <summary>The disclaimer shown below a performance metric value.</summary>
                [JsonProperty("varianceDisclaimer")]
                public virtual string VarianceDisclaimer { get; set; }

                /// <summary>The label shown above a bulleted list of warnings.</summary>
                [JsonProperty("warningHeader")]
                public virtual string WarningHeader { get; set; }

            }
        }

        /// <summary>A top-level error message that, if present, indicates a serious enough problem that this Lighthouse
        /// result may need to be discarded.</summary>
        public class RuntimeErrorData
        {
            /// <summary>The enumerated Lighthouse Error code.</summary>
            [JsonProperty("code")]
            public virtual string Code { get; set; }

            /// <summary>A human readable message explaining the error code.</summary>
            [JsonProperty("message")]
            public virtual string Message { get; set; }

        }

        public class StackPacksData
        {
            /// <summary>The stack pack advice strings.</summary>
            [JsonProperty("descriptions")]
            public virtual IDictionary<string, string> Descriptions { get; set; }

            /// <summary>The stack pack icon data uri.</summary>
            [JsonProperty("iconDataURL")]
            public virtual string IconDataURL { get; set; }

            /// <summary>The stack pack id.</summary>
            [JsonProperty("id")]
            public virtual string Id { get; set; }

            /// <summary>The stack pack title.</summary>
            [JsonProperty("title")]
            public virtual string Title { get; set; }

        }

        /// <summary>Timing information for this LHR.</summary>
        public class TimingData
        {
            /// <summary>The total duration of Lighthouse's run.</summary>
            [JsonProperty("total")]
            public virtual double? Total { get; set; }
        }
    }

    public class PagespeedApiLoadingExperienceV5
    {
        /// <summary>The url, pattern or origin which the metrics are on.</summary>
        [JsonProperty("id")]
        public virtual string Id { get; set; }

        [JsonProperty("initial_url")]
        public virtual string InitialUrl { get; set; }

        [JsonProperty("metrics")]
        public virtual IDictionary<string, MetricsDataElement> Metrics { get; set; }

        [JsonProperty("overall_category")]
        public virtual string OverallCategory { get; set; }

        /// <summary>The ETag of the item.</summary>
        public virtual string ETag { get; set; }


        /// <summary>The type of the metric.</summary>
        public class MetricsDataElement
        {
            [JsonProperty("category")]
            public virtual string Category { get; set; }

            [JsonProperty("distributions")]
            public virtual IList<MetricsDataElement.DistributionsData> Distributions { get; set; }

            [JsonProperty("percentile")]
            public virtual int? Percentile { get; set; }

            public class DistributionsData
            {
                [JsonProperty("max")]
                public virtual int? Max { get; set; }

                [JsonProperty("min")]
                public virtual int? Min { get; set; }

                [JsonProperty("proportion")]
                public virtual double? Proportion { get; set; }
            }
        }
    }

    public class PageSpeedApiResponseV5
    {
        public PageSpeedApiResponseV5()
        {
            LighthouseResult = new LighthouseResultV5();
        }

        /// <summary>The UTC timestamp of this analysis.</summary>
        [JsonProperty("analysisUTCTimestamp")]
        public virtual string AnalysisUTCTimestamp { get; set; }

        /// <summary>The captcha verify result</summary>
        [JsonProperty("captchaResult")]
        public virtual string CaptchaResult { get; set; }

        /// <summary>Canonicalized and final URL for the document, after following page redirects (if any).</summary>
        [JsonProperty("id")]
        public virtual string Id { get; set; }

        /// <summary>Kind of result.</summary>
        [JsonProperty("kind")]
        public virtual string Kind { get; set; }

        /// <summary>Lighthouse response for the audit url as an object.</summary>
        [JsonProperty("lighthouseResult")]
        public virtual LighthouseResultV5 LighthouseResult { get; set; }

        /// <summary>Metrics of end users' page loading experience.</summary>
        [JsonProperty("loadingExperience")]
        public virtual PagespeedApiLoadingExperienceV5 LoadingExperience { get; set; }

        /// <summary>Metrics of the aggregated page loading experience of the origin</summary>
        [JsonProperty("originLoadingExperience")]
        public virtual PagespeedApiLoadingExperienceV5 OriginLoadingExperience { get; set; }

        /// <summary>The version of PageSpeed used to generate these results.</summary>
        [JsonProperty("version")]
        public virtual VersionData Version { get; set; }

        /// <summary>The ETag of the item.</summary>
        public virtual string ETag { get; set; }


        /// <summary>The version of PageSpeed used to generate these results.</summary>
        public class VersionData
        {
            /// <summary>The major version number of PageSpeed used to generate these results.</summary>
            [JsonProperty("major")]
            public virtual int? Major { get; set; }

            /// <summary>The minor version number of PageSpeed used to generate these results.</summary>
            [JsonProperty("minor")]
            public virtual int? Minor { get; set; }

        }
    }
}
