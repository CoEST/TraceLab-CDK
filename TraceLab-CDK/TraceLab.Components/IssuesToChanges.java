package VSM;

import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileReader;
import java.util.ArrayList;
import java.util.Iterator;

import org.json.simple.JSONArray;
import org.json.simple.JSONObject;
import org.json.simple.parser.JSONParser;

import jdk.internal.org.objectweb.asm.util.CheckAnnotationAdapter;

public class IssuesToChanges {
	private ArrayList<SourceCodeEntry> Source;
	private String file_name;
	private String ch;
	
	public IssuesToChanges(File f) throws FileNotFoundException {
		this.Source = new ArrayList<SourceCodeEntry>();
		this.ParseIssuesChange(f.getAbsolutePath());
	}
	
	public void ParseIssuesChange (String filename) throws FileNotFoundException {
		 FileReader reader = new FileReader(new File(filename));
		 
		 JSONParser jsonParser = new JSONParser();	 
		 try {
			 Object obj = jsonParser.parse(reader);
			 JSONObject jsonObject = (JSONObject) obj;
			 
			 for(Iterator iterator = jsonObject.keySet().iterator(); iterator.hasNext();) {
				    file_name = (String) iterator.next();
				    SourceCodeEntry sc = new SourceCodeEntry(file_name);
				    JSONArray jsonArray = (JSONArray) jsonObject.get(file_name);
				    for(int i=0; i<jsonArray.size(); i++) {
						 JSONObject jObject = (JSONObject) jsonArray.get(i);
						 ch = (String) jObject.get("commit_hash");
						 CodeHashEntry che = new CodeHashEntry(ch);
						 sc.addHash(che);
				    }
				    Source.add(sc);
			}
		 } catch (Exception e) {
			 e.printStackTrace();
		 }
	}
	
	public ArrayList<SourceCodeEntry> getSource(){
		return this.Source;
	}
}
