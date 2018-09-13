import Layout from '../components/layout'
import withAuth from '../components/with-auth'

export default withAuth(
  (props) => (
    <Layout title="About" {...props}>
      <h1 id="catalogue">About the Catalogue</h1>
      <p>The Catalogue is in private beta.</p>

      <h2 id="nhs-digital">About NHS Digital</h2>
      <p>Find out more about NHS Digital <a href="https://digital.nhs.uk/about-nhs-digital">here</a></p>
    </Layout>
  )
)
