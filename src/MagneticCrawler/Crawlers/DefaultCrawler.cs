using System.ComponentModel.Composition;

namespace MagneticCrawler.Crawlers
{
    //[Export(WebCrawler.CrawlerContractString, typeof(WebCrawler))]
    public class DefaultCrawler : WebCrawler
    {
        public override void Start(string searchtext)
        {
            try
            {
                //string value = Request("http://btinfo.sandai.net/querybt.fcg?infoid=D8E3B1D297F82B419B2DB6D730FD0265214A31ED");
                //string value = Request("http://i.vod.xunlei.com/req_subBT/info_hash/D8E3B1D297F82B419B2DB6D730FD0265214A31ED/req_num/1000/req_offset/0/");
                //string value = Request("http://106.15.195.249:8011/search_new?q=%E9%92%A2%E9%93%81%E4%BE%A0&p=1");

                //百度搜索建议
                string value = WebRequestHelper.Request("http://suggestion.baidu.com/su?wd=钢铁&cb=window.baidu.sug");
            }
            catch (System.Exception e)
            {

            }
        }
    }
}
