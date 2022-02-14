pipeline{
    agent any
    
    stages{
        
        stage('Tests') {
            steps {
                script {
                    try {
                        bat "develop-run_tests_editmode.bat %PROJECT_ROOT% %BUILD_NAME% %BUILD_TARGET% %BUILD_FLAG% _unused_ testResults.xml"
                        bat "develop-run_tests_playmode.bat %PROJECT_ROOT% %BUILD_NAME% %BUILD_TARGET% %BUILD_FLAG% _unused_ testResults.xml"
                    }
                    catch (exc) {
                        script{
                            error("Build Failed Due to Test Fail.")
                        }
                    }
                }
            }
        }
      
        stage('Build') {
            steps {
                bat "develop-build_and_test.bat %PROJECT_ROOT% %BUILD_NAME% %BUILD_TARGET% %BUILD_FLAG% _unused_ testResults.xml"
            }
        }
        
        stage('Export') {
            steps {
                archiveArtifacts artifacts: 'Builds/', fingerprint: true   
            }
        }
    }
}
