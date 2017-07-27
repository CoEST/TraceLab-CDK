package VSM;

import org.eclipse.jgit.lib.ObjectId;
import org.eclipse.jgit.lib.ObjectLoader;
import org.eclipse.jgit.lib.ObjectStream;
import org.eclipse.jgit.lib.Repository;
import org.eclipse.jgit.revwalk.RevCommit;
import org.eclipse.jgit.revwalk.RevTree;
import org.eclipse.jgit.revwalk.RevWalk;
import org.eclipse.jgit.storage.file.FileRepositoryBuilder;
import org.eclipse.jgit.treewalk.TreeWalk;
import org.eclipse.jgit.treewalk.filter.PathFilter;

import java.io.*;

public class GitFileGetter {
    /**
     *
     * @param repositoryPath: absolute path to git repository, inclduing .git path
     * @param filePath: file path in the repository
     * @param sha: sha revision string
     * @return the file content for that version
     * @throws IOException
     */
    public static String getFileContent(String repositoryPath, String filePath, String sha) throws IOException {
        FileRepositoryBuilder builder = new FileRepositoryBuilder();
        Repository repository = builder.setGitDir(new File(repositoryPath)).setMustExist(true).build();

        // find the revision
        ObjectId lastCommitId = repository.resolve(sha);

        // now we have to get the commit
        RevWalk revWalk = new RevWalk(repository);
        RevCommit commit = revWalk.parseCommit(lastCommitId);

        // and using commit's tree find the path
        RevTree tree = commit.getTree();
        TreeWalk treeWalk = new TreeWalk(repository);
        treeWalk.addTree(tree);
        treeWalk.setRecursive(true);
        treeWalk.setFilter(PathFilter.create(filePath));
        if (!treeWalk.next()) {
            return null;
        }
        ObjectId objectId = treeWalk.getObjectId(0);
        ObjectLoader loader = repository.open(objectId);

        String fileContent;

        try (ObjectStream stream = loader.openStream()) {
            fileContent = readInputStreamToString(stream);
            stream.close();
        }

        System.out.println("------------------");
        System.out.println(fileContent);
        System.out.println("------------------");

        return fileContent;
    }

    private static final int BUFFER_SIZE = 16384;

    private static String readInputStreamToString(InputStream stream) throws IOException {
        final char[] buffer = new char[BUFFER_SIZE];

        Reader reader = new BufferedReader( new InputStreamReader( stream, "UTF-8"), BUFFER_SIZE );
        StringBuilder result = new StringBuilder(BUFFER_SIZE);

        int len;
        while((len = reader.read( buffer, 0, buffer.length )) >= 0) {
            result.append(buffer, 0, len);
        }

        return result.toString();
    }
}
