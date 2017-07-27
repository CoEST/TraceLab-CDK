package VSM;

import java.io.File;
import java.io.FileNotFoundException;
import java.util.ArrayList;
import java.util.Scanner;

public class Stopwords {
	public ArrayList<String> stopwords;
	
	public Stopwords() throws FileNotFoundException {
		this.getStopwords();
	}
	/* replace all stopwords with a empty string ""  for a single string*/
	public String removeEntire(String output) {
		for (String word : output.split("\\s+")) {
			if(stopwords.contains(word)){
				word = word.replace(word, "");
			}
		}
		return output;
	}

	
	/* replace a word that is a stopword with an empty string "" */
	public String removeSingle(String word){
		if(stopwords.contains(word)){
			word = word.replace(word, "");
		}
		return word;
	}
	
	public ArrayList<String> getStopwords() throws FileNotFoundException{
		Scanner s = new Scanner(new File("src/stopwords.txt"));
		stopwords = new ArrayList<String>();
		while (s.hasNext()){
		    stopwords.add(s.next());
		}
		s.close();
		return stopwords;
	}
}
