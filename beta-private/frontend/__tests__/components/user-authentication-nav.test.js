import { shallow } from 'enzyme'
import React from 'react'

import UserAuthenticationNav from '../../components/user-authentication-nav'

const T = UserAuthenticationNav

describe('UserAuthenticationNav component', () => {
  it('renders a group containing login link if the user is not authenticated', () => {
    let r = shallow(<T />)
    expect(r.find('.auth a').text()).toEqual('Log In')

    r = shallow(<T user={{ is_authenticated: false }} />)
    expect(r.find('.auth a').text()).toEqual('Log In')
  })

  it('renders the users name and a logout link if the user is authenticated', () => {
    let r = shallow(<T user={{ is_authenticated: true, first_name: 'Something!' }} />)
    expect(r.find('.auth .user').text()).toEqual(
      expect.stringContaining('Something!')
    )
    expect(r.find('.auth a[href="/logout"]').length).toBeTruthy()
  })
})

/* eslint-env jest */
