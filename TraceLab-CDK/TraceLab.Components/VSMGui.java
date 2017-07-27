package VSM;

import java.awt.EventQueue;
import java.awt.Font;

import javax.swing.JFrame;
import javax.swing.JButton;
import javax.swing.JFileChooser;
import javax.swing.JOptionPane;
import javax.swing.JScrollPane;
import javax.swing.JTextArea;
import java.awt.event.ActionListener;
import java.io.File;
import java.io.FileNotFoundException;
import java.util.ArrayList;
import java.util.Arrays;
import java.awt.event.ActionEvent;
import javax.swing.AbstractAction;
import javax.swing.Action;
import javax.swing.ButtonGroup;
import javax.swing.JCheckBox;
import javax.swing.JLabel;
import javax.swing.JRadioButton;

public class VSMGui {
	private JFrame frame;
	//private JTextField textField;
	private JTextArea fileList;
	private JTextArea linkFile;
	private JTextArea sourceCodeDirectory;
	private JTextArea issuesToChanges;
	private JTextArea changesToCode;
	private JCheckBox chckbxStopwords;
	private JCheckBox chckbxStemmer;
	private JFileChooser chooser;
	private JButton btnChangesToCode;
	private JRadioButton rdbtnYes;
	private JRadioButton rdbtnNo;
	private JScrollPane scrollPane_3;
	private static ArrayList<File> files;
	private static File linksFile;
	private static String codeDirectory;
	private static File ic;
	private static File cc;
	private static ArrayList<Document> documents;
	private static Corpus corpus;
	private static VectorSpaceModel vectorSpace;
	private Links knownLinks;
	private IssuesToChanges ITC;
	private ChangesToCode CTC;
	private ArrayList<LinkEntry> finalResult;
	ArrayList<SourceCodeEntry> source;
	private final Action action = new SwingAction();
	private final Action action_1 = new SwingAction_1();
	private final Action action_2 = new SwingAction_2();
	private final Action action_3 = new SwingAction_3();
	private final Action action_4 = new SwingAction_4();
	private final Action action_5 = new SwingAction_5();

	/**
	 * Launch the application.
	 */
	public static void main(String[] args) {
		EventQueue.invokeLater(new Runnable() {
			public void run() {
				try {
					VSMGui window = new VSMGui();
					window.frame.setVisible(true);
				} catch (Exception e) {
					e.printStackTrace();
				}
			}
		});
	}

	/**
	 * Create the application.
	 */
	public VSMGui() {
		initialize();
	}

	/**
	 * Initialize the contents of the frame.
	 */
	private void initialize() {
		frame = new JFrame();
		frame.setBounds(100, 100, 569, 426);
		frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
		frame.getContentPane().setLayout(null);
		
		JButton btnInputData = new JButton("Input Data");
		btnInputData.setAction(action);
		btnInputData.setBounds(40, 29, 121, 33);
		frame.getContentPane().add(btnInputData);
		
		JScrollPane scrollPane = new JScrollPane();
		scrollPane.setBounds(171, 29, 357, 42);
		frame.getContentPane().add(scrollPane);
		
		fileList = new JTextArea();
		scrollPane.setViewportView(fileList);
		fileList.setEditable(false);
		
		JButton btnInputKnownLinks = new JButton("Input Links");
		btnInputKnownLinks.setAction(action_1);
		btnInputKnownLinks.setBounds(40, 83, 121, 33);
		frame.getContentPane().add(btnInputKnownLinks);
		
		/*JLabel lblThresholdValue = new JLabel("Threshold Value");
		lblThresholdValue.setBounds(50, 169, 111, 33);
		frame.getContentPane().add(lblThresholdValue);
		
		textField = new JTextField();
		textField.setText("0.10");
		textField.setBounds(194, 175, 86, 20);
		frame.getContentPane().add(textField);
		textField.setColumns(10);*/
		
		JScrollPane scrollPane_1 = new JScrollPane();
		scrollPane_1.setBounds(171, 82, 357, 36);
		frame.getContentPane().add(scrollPane_1);
		
		linkFile = new JTextArea();
		scrollPane_1.setViewportView(linkFile);
		linkFile.setEditable(false);
		
		JButton btnAnalyze = new JButton("Analyze");
		btnAnalyze.setAction(action_2);
		btnAnalyze.setBounds(341, 305, 160, 50);
		frame.getContentPane().add(btnAnalyze);
		
		chckbxStopwords = new JCheckBox("Stopwords");
		chckbxStopwords.setSelected(true);
		chckbxStopwords.setBounds(40, 312, 97, 23);
		frame.getContentPane().add(chckbxStopwords);
		
		chckbxStemmer = new JCheckBox("Stemmer");
		chckbxStemmer.setSelected(true);
		chckbxStemmer.setBounds(151, 312, 97, 23);
		frame.getContentPane().add(chckbxStemmer);
		
		JLabel lblIncludeSourceCode = new JLabel("Include Source Code");
		lblIncludeSourceCode.setBounds(40, 129, 121, 23);
		frame.getContentPane().add(lblIncludeSourceCode);
		
		JButton btnSourceCode = new JButton("Source Code Directory");
		btnSourceCode.setAction(action_3);
		btnSourceCode.setBounds(40, 175, 148, 33);
		btnSourceCode.setFont(new Font("Arial", Font.BOLD, 10));
		frame.getContentPane().add(btnSourceCode);
		
		JScrollPane scrollPane_4 = new JScrollPane();
		scrollPane_4.setBounds(198, 175, 313, 33);
		frame.getContentPane().add(scrollPane_4);
		
		sourceCodeDirectory = new JTextArea();
		scrollPane_4.setViewportView(sourceCodeDirectory);
		sourceCodeDirectory.setEditable(false);
				
		JButton btnIssuesToChanges = new JButton("Issues-to-Changes");
		btnIssuesToChanges.setAction(action_4);
		btnIssuesToChanges.setFont(new Font("Arial", Font.BOLD, 10));
		btnIssuesToChanges.setBounds(40, 219, 148, 33);
		frame.getContentPane().add(btnIssuesToChanges);
		
		JScrollPane scrollPane_2 = new JScrollPane();
		scrollPane_2.setBounds(198, 218, 313, 33);
		frame.getContentPane().add(scrollPane_2);
		
		issuesToChanges = new JTextArea();
		scrollPane_2.setViewportView(issuesToChanges);
		issuesToChanges.setEditable(false);
		
		btnChangesToCode = new JButton("Changes-to-Code");
		btnChangesToCode.setAction(action_5);
		btnChangesToCode.setFont(new Font("Arial", Font.BOLD, 10));
		btnChangesToCode.setBounds(40, 262, 148, 32);
		btnChangesToCode.setEnabled(false);
		frame.getContentPane().add(btnChangesToCode);
		
		scrollPane_3 = new JScrollPane();
		scrollPane_3.setBounds(198, 261, 313, 33);
		scrollPane_3.setVisible(false);
		frame.getContentPane().add(scrollPane_3);
		
		changesToCode = new JTextArea();
		scrollPane_3.setViewportView(changesToCode);
		changesToCode.setVisible(false);
		changesToCode.setEditable(false);
		
		ButtonGroup bgroup = new ButtonGroup();
		rdbtnYes = new JRadioButton("Yes");
		rdbtnYes.addActionListener(new ActionListener() {
			public void actionPerformed(ActionEvent e) {
				btnSourceCode.setEnabled(true);
				scrollPane_4.setVisible(true);
				sourceCodeDirectory.setVisible(true);
				btnIssuesToChanges.setEnabled(true);
				scrollPane_2.setVisible(true);
				issuesToChanges.setVisible(true);
				btnChangesToCode.setEnabled(false);
				scrollPane_3.setVisible(false);
				changesToCode.setVisible(false);
			}
		});
		rdbtnNo = new JRadioButton("No");
		rdbtnNo.addActionListener(new ActionListener() {
			public void actionPerformed(ActionEvent e) {
				btnSourceCode.setEnabled(false);
				scrollPane_4.setVisible(false);
				sourceCodeDirectory.setVisible(false);
				btnIssuesToChanges.setEnabled(false);
				scrollPane_2.setVisible(false);
				issuesToChanges.setVisible(false);
				btnChangesToCode.setEnabled(false);
				scrollPane_3.setVisible(false);
				changesToCode.setVisible(false);
			}
		});
		bgroup.add(rdbtnYes);
		rdbtnYes.setBounds(171, 129, 51, 23);
		rdbtnYes.setSelected(true);
		frame.getContentPane().add(rdbtnYes);
		bgroup.add(rdbtnNo);
		rdbtnNo.setBounds(224, 129, 51, 23);
		frame.getContentPane().add(rdbtnNo);
	}
	
	private class SwingAction extends AbstractAction {
		public SwingAction() {
			putValue(NAME, "Input Data");
			putValue(SHORT_DESCRIPTION, "Select all JSON files you wish to analyze");
		}
		public void actionPerformed(ActionEvent e) {
			fileList.setText("");
			chooser = new JFileChooser(); 
		    chooser.setCurrentDirectory(new java.io.File("."));
		    chooser.setDialogTitle("Select files to analyze");
		    chooser.setMultiSelectionEnabled(true);
		    chooser.setFileSelectionMode(JFileChooser.FILES_AND_DIRECTORIES);
		    chooser.setAcceptAllFileFilterUsed(false);
		    if (chooser.showOpenDialog(frame) == JFileChooser.APPROVE_OPTION) {
		      files = new ArrayList<File>(Arrays.asList(chooser.getSelectedFiles()));
		      for(int i = 0; i<files.size(); i++) {
		    	  if(!fileList.getText().toString().contains(files.get(i).getPath())) {
		    		  fileList.append(files.get(i).getPath()+'\n');
		    	  }
		      }
		    }
		    else {
		      System.out.println("No Selection ");
		      }
		    String message = "Would you like to upload more files to analyze?";
			int reply = JOptionPane.showConfirmDialog(null, message, null, JOptionPane.YES_NO_OPTION);
			if  (reply == JOptionPane.YES_OPTION) {
				do {
					if (chooser.showOpenDialog(frame) == JFileChooser.APPROVE_OPTION) {
						  ArrayList<File> tempList = new ArrayList<File>(Arrays.asList(chooser.getSelectedFiles()));
						  for(int i = 0; i<tempList.size(); i++) {
							  if(!files.contains(tempList.get(i))) files.add(tempList.get(i));
						  }
					      for(int i = 0; i<files.size(); i++) {
					    	  if(!fileList.getText().toString().contains(files.get(i).getPath())) {
					    		  fileList.append(files.get(i).getPath()+'\n');				    		  
					    	  }
					      }
					    }
					reply = JOptionPane.showConfirmDialog(null, message, null, JOptionPane.YES_NO_OPTION);
				} while  (reply == JOptionPane.YES_OPTION);
			}
		}
	}
	
	private class SwingAction_1 extends AbstractAction {
		public SwingAction_1() {
			putValue(NAME, "Input Links");
			putValue(SHORT_DESCRIPTION, "Input JSON file containing known links in project");
		}
		public void actionPerformed(ActionEvent e) {
			linkFile.setText("");
			chooser = new JFileChooser(); 
		    chooser.setCurrentDirectory(new java.io.File("."));
		    chooser.setDialogTitle("Select file containing known links");
		    chooser.setMultiSelectionEnabled(true);
		    chooser.setFileSelectionMode(JFileChooser.FILES_ONLY);
		    chooser.setAcceptAllFileFilterUsed(false);
		    if (chooser.showOpenDialog(frame) == JFileChooser.APPROVE_OPTION) { 
		      linksFile = chooser.getSelectedFile();
		      if(linkFile.getText().isEmpty()) linkFile.append(linksFile.getPath());
		      try {
				knownLinks = new Links(linksFile);
		      } catch (FileNotFoundException e1) {
				e1.printStackTrace();
		      }
		    }
		    else {
		      System.out.println("No Selection ");
		      }
		    }
	}
	
	private class SwingAction_2 extends AbstractAction {
		public SwingAction_2() {
			putValue(NAME, "Analyze");
			putValue(SHORT_DESCRIPTION, "Click to run the VSM model on the selected data");
		}
		public void actionPerformed(ActionEvent e) {
			if(!fileList.getText().isEmpty() && files.size() >= 2) {
				//double result = Double.parseDouble(textField.getText());
				documents = new ArrayList<Document>();
				finalResult = new ArrayList<LinkEntry>();
				for(int i = 0; i<files.size(); i++) {
					Document document;
					if(rdbtnNo.isSelected()) document = new Document(files.get(i).getAbsolutePath());
					else document = new Document(files.get(i).getAbsolutePath(), source, codeDirectory);
					if(chckbxStopwords.isSelected()) {
						document.stopwords_selected = true;
					}
					if(chckbxStemmer.isSelected()) {
						document.stemmer_selected = true;
					}
					documents.add(document);
					}
				corpus = new Corpus(documents);
				vectorSpace = new VectorSpaceModel(corpus);
				for(int i = 0; i < documents.size(); i++) {
					for(int j = i+1; j < documents.size(); j++){
						if(i+1 >= documents.size()) return;
						Document doc1 = documents.get(i);
						Document doc2 = documents.get(j);
						double cs = vectorSpace.cosineSimilarity(doc1, doc2);
						//if(cs >= result) {
							System.out.println("Comparing " + doc1.getFileName() + " and " + doc2.getFileName() + ": " + cs);
							LinkEntry entry = new LinkEntry();
							String d1 = doc1.removeExtension();
							String d2 = doc2.removeExtension();
							entry.setVariables(d1 , d2);
							entry.setScore(cs);
							if(!linkFile.getText().isEmpty()) {
								if(listContains(d1, d2, knownLinks.getLinks()) == true) {
									entry.setBoolean();
								}
							}
							finalResult.add(entry);
						//}
					}
				}
				JOptionPane.showMessageDialog(frame.getComponent(0), "Analysis complete");
				frame.dispose();
			}
			else JOptionPane.showMessageDialog (null, "Must select at least two files to analyze", null, JOptionPane.WARNING_MESSAGE);
		}
	}
	
	private class SwingAction_3 extends AbstractAction {
		public SwingAction_3() {
			putValue(NAME, "Source Code Directory");
			putValue(SHORT_DESCRIPTION, "Select the local directory containing source code for the project being analyzed");
		}
		public void actionPerformed(ActionEvent e) {
			sourceCodeDirectory.setText("");
			chooser = new JFileChooser(); 
		    chooser.setCurrentDirectory(new java.io.File("."));
		    chooser.setDialogTitle("Select local directory containing source code");
		    chooser.setMultiSelectionEnabled(true);
		    chooser.setFileSelectionMode(JFileChooser.DIRECTORIES_ONLY);
		    chooser.setAcceptAllFileFilterUsed(false);   
		    if (chooser.showOpenDialog(frame) == JFileChooser.APPROVE_OPTION) { 
		      codeDirectory = chooser.getSelectedFile().getPath();
		      if(!codeDirectory.isEmpty() && sourceCodeDirectory.getText().isEmpty()) sourceCodeDirectory.append(codeDirectory);
		    }
		    else {
		      System.out.println("No Selection ");
		      }
		   }
	}
	
	private class SwingAction_4 extends AbstractAction {
		public SwingAction_4() {
			putValue(NAME, "Issues_to_Changes");
			putValue(SHORT_DESCRIPTION, "Select the _issues_to_change_set file associated with the project");
		}
		public void actionPerformed(ActionEvent e) {
			issuesToChanges.setText("");
			chooser = new JFileChooser(); 
		    chooser.setCurrentDirectory(new java.io.File("."));
		    chooser.setDialogTitle("Select file _issues_to_change_set for the project");
		    chooser.setMultiSelectionEnabled(true);
		    chooser.setFileSelectionMode(JFileChooser.FILES_ONLY);
		    chooser.setAcceptAllFileFilterUsed(false);
		    if (chooser.showOpenDialog(frame) == JFileChooser.APPROVE_OPTION) { 
		      ic = chooser.getSelectedFile();
		      if(issuesToChanges.getText().isEmpty()) issuesToChanges.append(ic.getPath());
		      try {
				ITC = new IssuesToChanges(ic);
				btnChangesToCode.setEnabled(true);
				scrollPane_3.setVisible(true);
				changesToCode.setVisible(true);
		      } catch (FileNotFoundException e1) {
				e1.printStackTrace();
		      }
		    }
		    else {
		      System.out.println("No Selection ");
		      }
		}
	}
	
	private class SwingAction_5 extends AbstractAction {
		public SwingAction_5() {
			putValue(NAME, "Changes_to_Code");
			putValue(SHORT_DESCRIPTION, "Select the _change_set_to_code file associated with the project");
		}
		public void actionPerformed(ActionEvent e) {
			changesToCode.setText("");
			chooser = new JFileChooser(); 
		    chooser.setCurrentDirectory(new java.io.File("."));
		    chooser.setDialogTitle("Select file _change_set_to_code for the project");
		    chooser.setMultiSelectionEnabled(true);
		    chooser.setFileSelectionMode(JFileChooser.FILES_ONLY);
		    chooser.setAcceptAllFileFilterUsed(false);
		    if (chooser.showOpenDialog(frame) == JFileChooser.APPROVE_OPTION) { 
		      cc = chooser.getSelectedFile();
		      if(changesToCode.getText().isEmpty()) changesToCode.append(cc.getPath());
		      try {
					CTC = new ChangesToCode(cc, ITC.getSource());
					source = ITC.getSource();
			      } catch (FileNotFoundException e1) {
					e1.printStackTrace();
			      }
		    }
		    else {
		      System.out.println("No Selection ");
		      }
		}
	}
	
	private boolean listContains(String t, String s, ArrayList<LinkEntry> a) {
		for(int i=0; i<a.size(); i++){
			if(a.get(i).getSource().equals(t)) {
				if(a.get(i).getTarget().equals(s))
					return true;
			}
			else if(a.get(i).getSource().equals(s)) {
				if(a.get(i).getTarget().equals(t))
					return true;
			}
			else
				return false;
		}
		return false;
	}
	
	public ArrayList<LinkEntry> returnResults(){
		return finalResult;
	}
}
