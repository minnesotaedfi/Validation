node{
      stage ('Checkout'){
    checkout scm
    }

      stage ('Build'){
    bat 'nuget restore validation.sln'
    bat 'msbuild validation.sln /p:Configuration=Release'
    }

      stage ('Archive'){
    archive '$WORKSPACE/build/Release/**'
    }
}
