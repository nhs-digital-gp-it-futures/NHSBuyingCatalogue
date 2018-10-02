import React from 'react'
import Link from 'next/link'
import Layout from '../../components/layout'
import { suppliersOnly } from '../../components/with-auth'
import { ApiClient, Solutions } from '../../lib/catalogue-api'

const SupplierHomepage = suppliersOnly(
  ({ solutions, ...props }) => (
    <Layout {...props} title='Supplier Home'>
      <h1>Supplier Home</h1>
      <p>You have logged in from supplier organisation {props.user.org.name}</p>
      <section id='onboarding-solutions'>
        <header>
          <h2>Onboarding Solutions</h2>
        </header>
        <OnboardingSolutions solutions={solutions} />
      </section>
      <section id='live-solutions'>
        <header>
          <h2>Live Solutions</h2>
        </header>
        <LiveSolutions solutions={solutions} />
      </section>
    </Layout>
  )
)

SupplierHomepage.getInitialProps = async ({ req }) => {
  if (req) {
    const api = new ApiClient.SolutionsApi()
    const result = await api.apiSolutionsByOrganisationByOrganisationIdGet(req.user.org.id, {
      pageSize: 9999
    })
    return {
      solutions: result.items
    }
  }
}

const solutionIsOnboarding = (soln) => (
  soln.status !== Solutions.StatusEnum.Failed &&
  soln.status !== Solutions.StatusEnum.Approved
)

const solutionIsLive = (soln) => soln.status === Solutions.StatusEnum.Approved

const OnboardingSolutions = (props) => (
  <FilteredSolutions {...props} filterFn={solutionIsOnboarding} />
)

const LiveSolutions = (props) => (
  <FilteredSolutions {...props} filterFn={solutionIsLive} />
)

const FilteredSolutions = ({ solutions, filterFn, ...props }) => (
  <SolutionsTable {...props} solutions={solutions.filter(filterFn)} />
)

const SolutionsTable = ({ solutions }) => (
  <table>
    <thead>
      <tr>
        <th>Name</th>
        <th>Status</th>
      </tr>
    </thead>
    <tbody>{
      solutions.map(soln => (
        <tr>
          <td>{soln.name}</td>
          <td>{soln.status}</td>
        </tr>
      ))
    }
    </tbody>
  </table>
)
