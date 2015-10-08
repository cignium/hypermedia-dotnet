namespace Hypermedia {
    public class DocumentResult : HypermediaResult<DocumentResult> {
        public override string Profile => "document";
        protected override DocumentResult Self => this;
    }
}